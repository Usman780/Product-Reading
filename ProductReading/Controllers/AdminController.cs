using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProductReading.Models;
using ProductReading.BL;
using ProductReading.Helping_Classes;
using System.IO;
using System.Net;
using Microsoft.AspNet.SignalR;
using ProductReading.DataHub;
using System.IO.Compression;

namespace ProductReading.Controllers
{
    [ValidationFilter(Role = 1)]
    public class AdminController : Controller
    {
        private GeneralPurpose gp = new GeneralPurpose();
        private DatabaseEntities de = new DatabaseEntities();

        public ActionResult Index(string msg = "", string color = "black", string way = "")
        {
            ViewBag.ProductCount = new ProductRecordBL().GetActiveProductsList(de).Count();

            ViewBag.Message = msg;
            ViewBag.Color = color;
            ViewBag.Way = way;

            return View();
        }


        public ActionResult ViewProduct(string msg = "", string color = "black", string way = "")
        {
            int isRefresh = 0;
            CommonTable ct = new ProductRecordBL().GetLastRefresh(de);

            if (ct != null)
            {
                TimeSpan ts = GeneralPurpose.DateTimeNow() - Convert.ToDateTime(ct.RefreshTime);
                if(ts.TotalDays >= ProjectVariable.RefresDays)
                {
                    isRefresh = 1;
                }
            }

            ViewBag.IsRefresh = isRefresh;
            ViewBag.CommonTable = ct;

            ViewBag.Message = msg;
            ViewBag.Color = color;
            ViewBag.Way = way;

            return View();
        }

        public ActionResult RefreshProductImage(int id, string way = "")
        {
            ProductRecord pr = new ProductRecordBL().GetActiveProductById(id, de);
            if (pr == null)
            {
                return RedirectToAction("ViewProduct", "Admin", new { msg = "Record not found", color = "red", way = way });
            }

            pr.ProductImagePath = GetFtpImage(pr.Product_SKU, Convert.ToDateTime(pr.CreatedAt), pr.Small_Image_File);

            if (pr.ProductImagePath == null)
            {
                return RedirectToAction("ViewProduct", "Admin", new { msg = "Image not found", color = "red", way = way });
            }

            bool chkProduct = new ProductRecordBL().UpdateProduct(pr, de);

            if (chkProduct)
            {
                return RedirectToAction("ViewProduct", "Admin", new { msg = "Record updated successfully", color = "green", way = way });
            }
            else
            {
                return RedirectToAction("ViewProduct", "Admin", new { msg = "Somethings' wrong", color = "red", way = way });
            }
        }


        [HttpPost]
        public ActionResult GetProductCatalog()
        {
            DateTime dt = GeneralPurpose.DateTimeNow();

            CommonTable ct = new ProductRecordBL().GetLastRefresh(de);
            if (ct == null)
            {
                CommonTable ct2 = new CommonTable()
                {
                    RefreshTime = dt,
                    IsActive = 1,
                    CreatedAt = GeneralPurpose.DateTimeNow()
                };

                bool chkRefresh2 = new ProductRecordBL().AddLastRefresh(ct2, de);

                if (!chkRefresh2)
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

            var context = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();
            
            List<string> txtLines = GetFtpDataList("ProductData/Kroll_Complete_Catalog.txt");

            if(txtLines.Count == 0)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            if(txtLines.Count > 0 && txtLines[0] == "0")
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

            string[] columns;
            int counter = 0;
            int totalRows = txtLines.Count();
            foreach (string line in txtLines)
            {
                try
                {
                    columns = line.Split('\t');

                    if (counter != 0)
                    {
                        ProductRecord obj = new ProductRecord()
                        {
                            IsNew = 0,
                            Product_SKU = columns[0],
                            Product_Group = columns[1],
                            Manufacturer = columns[2],
                            Product_Type = columns[3],
                            Product_Name = columns[4],
                            Short_Description = columns[5],
                            Long_Description = columns[6],
                            Level_1_Category = columns[7],
                            Level_2_Category = columns[8],
                            UPC_Code = columns[9],
                            MAP_Price = columns[10],
                            Kroll_Sell = columns[11],
                            MSRP = columns[12],
                            Product_Weight = columns[13],
                            Small_Image_File = columns[14],
                            Small_Image_Text = columns[15],
                            Manu_Part_Number = columns[16],
                            Hazardous = columns[17],
                            Restricted = columns[18],
                            Complete_SKU = columns[19],
                            SKU_Variation = columns[20],
                            Parent_SKU = columns[21],
                            Choice1 = columns[22],
                            Option1 = columns[23],
                            Choice2 = columns[24],
                            Option2 = columns[25],
                            Choice3 = columns[26],
                            Option3 = columns[27],
                            Choice4 = columns[28],
                            Option4 = columns[29],
                            Choice5 = columns[30],
                            Option5 = columns[31],
                            Choice6 = columns[32],
                            Option6 = columns[33],
                            Choice7 = columns[34],
                            Option7 = columns[35],
                            Choice8 = columns[36],
                            Option8 = columns[37],
                            Choice9 = columns[38],
                            Option9 = columns[39],
                            Choice10 = columns[40],
                            Option10 = columns[41],
                            QTY_Available = "0",
                            Country_of_Origin_Code = columns[42],
                            IsActive = 1,
                            CreatedAt = dt
                        };

                        obj.ProductImagePath = GetFtpImage(obj.Product_SKU, dt, obj.Small_Image_File);

                        bool chkProd = new ProductRecordBL().AddProduct(obj, de);
                    
                        context.Clients.All.broadcastProgress(counter, totalRows, 2);
                        context.Clients.All.broadcastProgressIndex(counter, totalRows, 2);
                    }
                    counter++;

                }
                catch
                {
                    counter++;
                    continue;
                }
            }

            context.Clients.All.broadcastProgress(counter, totalRows, 1);
            context.Clients.All.broadcastProgressIndex(counter, totalRows, 1);

            return Json(1, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RefreshProduct()
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();

            DateTime dt = GeneralPurpose.DateTimeNow();

            CommonTable ct = new ProductRecordBL().GetLastRefresh(de);
            if (ct != null)
            {
                TimeSpan ts = dt - Convert.ToDateTime(ct.RefreshTime);

                if (ts.TotalDays >= ProjectVariable.RefresDays)
                {
                    ct.RefreshTime = dt;

                    bool chkRefresh = new ProductRecordBL().UpdateLastRefresh(ct, de);
                    if (!chkRefresh)
                    {
                        return Json(0, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                CommonTable ct2 = new CommonTable()
                {
                    RefreshTime = dt,
                    IsActive = 1,
                    CreatedAt = GeneralPurpose.DateTimeNow()
                };

                bool chkRefresh2 = new ProductRecordBL().AddLastRefresh(ct2, de);

                if (!chkRefresh2)
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }


            List<ProductRecord> newlist = new ProductRecordBL().GetNewProducts(de);

            foreach(ProductRecord p in newlist)
            {
                bool isClear = new ProductRecordBL().ClearNewProduct(p, de);
            }
            

            List<string> prodLines = GetFtpDataList("ProductData/Kroll_Complete_Catalog.txt");
            
            string[] columns;
            int counter = 0;
            int totalRows = prodLines.Count();
            foreach (string line in prodLines)
            {
                try
                {
                    columns = line.Split('\t');

                    if (counter != 0)
                    {
                        ProductRecord pr = new ProductRecordBL().GetActiveProductBySearch(columns[0], columns[10], de);

                        if (pr == null)
                        {
                            ProductRecord obj = new ProductRecord()
                            {
                                IsNew = 1,
                                Product_SKU = columns[0],
                                Product_Group = columns[1],
                                Manufacturer = columns[2],
                                Product_Type = columns[3],
                                Product_Name = columns[4],
                                Short_Description = columns[5],
                                Long_Description = columns[6],
                                Level_1_Category = columns[7],
                                Level_2_Category = columns[8],
                                UPC_Code = columns[9],
                                MAP_Price = columns[10],
                                Kroll_Sell = columns[11],
                                MSRP = columns[12],
                                Product_Weight = columns[13],
                                Small_Image_File = columns[14],
                                Small_Image_Text = columns[15],
                                Manu_Part_Number = columns[16],
                                Hazardous = columns[17],
                                Restricted = columns[18],
                                Complete_SKU = columns[19],
                                SKU_Variation = columns[20],
                                Parent_SKU = columns[21],
                                Choice1 = columns[22],
                                Option1 = columns[23],
                                Choice2 = columns[24],
                                Option2 = columns[25],
                                Choice3 = columns[26],
                                Option3 = columns[27],
                                Choice4 = columns[28],
                                Option4 = columns[29],
                                Choice5 = columns[30],
                                Option5 = columns[31],
                                Choice6 = columns[32],
                                Option6 = columns[33],
                                Choice7 = columns[34],
                                Option7 = columns[35],
                                Choice8 = columns[36],
                                Option8 = columns[37],
                                Choice9 = columns[38],
                                Option9 = columns[39],
                                Choice10 = columns[40],
                                Option10 = columns[41],
                                QTY_Available = "0",
                                Country_of_Origin_Code = columns[42],
                                IsActive = 1,
                                CreatedAt = dt
                            };

                            obj.ProductImagePath = GetFtpImage(obj.Product_SKU, dt, obj.Small_Image_File);

                            bool chkProd = new ProductRecordBL().AddProduct(obj, de);
                        }
                        else
                        {
                            if (pr.MAP_Price != columns[10])
                            {
                                pr.IsNew = 1;
                                pr.MAP_Price = columns[10];

                                bool chkProd = new ProductRecordBL().UpdateProduct(pr, de);
                            }
                        }

                        context.Clients.All.broadcastProgress(counter, totalRows, 2);
                        context.Clients.All.broadcastProgressIndex(counter, totalRows, 2);
                    }

                    counter++;
                }
                catch
                {
                    counter++;
                    continue;
                }
            }

            List<string> inventoryLines = GetFtpDataList("ProductData/Kroll_Stock_Inventory.txt");

            columns = null;
            counter = 0;
            totalRows = inventoryLines.Count();
            foreach (string line in inventoryLines)
            {
                try
                {
                    columns = line.Split('\t');

                    if (counter != 0)
                    {
                        ProductRecord pr = new ProductRecordBL().GetActiveProductBySku(columns[0], de);
                        if (pr != null)
                        {
                            pr.QTY_Available = columns[1];

                            bool chkProd = new ProductRecordBL().UpdateProduct(pr, de);
                        }

                        context.Clients.All.broadcastInventoryProgress(counter, totalRows, 2);
                        context.Clients.All.broadcastInventoryProgressIndex(counter, totalRows, 2);
                    }
                    counter++;

                }
                catch
                {
                    counter++;
                    continue;
                }
            }

            context.Clients.All.broadcastProgress(counter, totalRows, 1);
            context.Clients.All.broadcastProgressIndex(counter, totalRows, 1);

            return Json(1, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteProduct(int id, string way = "")
        {
            ProductRecord p = new ProductRecordBL().GetActiveProductById(id, de);
            if (p == null)
            {
                return RedirectToAction("ViewProduct", "Admin", new { msg = "Record not found", color = "red", way = way });
            }
            p.IsActive = 0;

            bool chkProduct = new ProductRecordBL().UpdateProduct(p, de);

            if (chkProduct)
            {
                return RedirectToAction("ViewProduct", "Admin", new { msg = "Record deleted successfully", color = "green", way = way });
            }
            else
            {
                return RedirectToAction("ViewProduct", "Admin", new { msg = "Somethings' wrong", color = "red", way = way });
            }
        }


        #region FTP Management

        public List<string> GetFtpDataList(string filePath)
        {
            List<string> txtLines = new List<string>();

            try
            {
                /* Create an FTP Request */
                FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create(ProjectVariable.FtpHost + "/" + filePath);
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential(ProjectVariable.FtpUsername, ProjectVariable.FtpPassword);
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Timeout = -1; //infinite timeout (currently not working)
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                /* Establish Return Communication with the FTP Server */
                FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                /* Get the FTP Server's Response Stream */
                Stream ftpStream = ftpResponse.GetResponseStream();

                using (StreamReader file = new StreamReader(ftpStream))
                {
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        txtLines.Add(line);
                    }

                    file.Close();
                }

                return txtLines;
            }
            catch
            {
                txtLines.Add("0");

                return txtLines;
            }
        }

        public string GetFtpImage(string sku, DateTime directory, string imgPath)
        {
            try
            {
                FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create(ProjectVariable.FtpHost + "/" + imgPath);
                ftpRequest.Credentials = new NetworkCredential(ProjectVariable.FtpUsername, ProjectVariable.FtpPassword);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Timeout = -1; //infinite timeout
                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                Stream ftpStream = ftpResponse.GetResponseStream();

                int pos = imgPath.LastIndexOf(".") + 1;
                string imgExt = "." + imgPath.Substring(pos, imgPath.Length - pos);

                string imgName = sku + "(" + GeneralPurpose.DateTimeNow().Ticks + ")" + imgExt;

                string newDir = "../Content/" + directory.Ticks.ToString();

                bool exists = System.IO.Directory.Exists(Server.MapPath(newDir));

                if (!exists)
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(newDir));
                }

                string finalPath = Path.Combine(Path.Combine(Server.MapPath(newDir), imgName));
                using (FileStream outputFileStream = new FileStream(finalPath, FileMode.Create))
                {
                    ftpStream.CopyTo(outputFileStream);
                }

                return newDir + "/" + imgName;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion


        #region Manage Zip Upload

        //public ActionResult UploadFile(string msg = "", string color = "black", string way = "")
        //{
        //    ViewBag.ProductCount = new ProductRecordBL().GetActiveProductsList(de).Count();

        //    ViewBag.Message = msg;
        //    ViewBag.Color = color;
        //    ViewBag.Way = way;

        //    return View();
        //}

        //[HttpPost]
        //public ActionResult PostUploadFile(HttpPostedFileBase postedFile)
        //{
        //    string rootDir = "../Content/test/" + GeneralPurpose.DateTimeNow().Ticks.ToString();

        //    bool exists = System.IO.Directory.Exists(Server.MapPath(rootDir));

        //    if (!exists)
        //    {
        //        System.IO.Directory.CreateDirectory(Server.MapPath(rootDir));
        //    }

        //    using (ZipArchive archive = new ZipArchive(postedFile.InputStream))
        //    {
        //        foreach (ZipArchiveEntry entry in archive.Entries)
        //        {
        //            int length = (int)entry.Length;
        //            string fullpath = entry.FullName;
        //            string name = entry.Name;

        //            if(name == "")
        //            {
        //                string childDir = rootDir +"/" + fullpath;

        //                bool childExists = System.IO.Directory.Exists(Server.MapPath(childDir));

        //                if (!childExists)
        //                {
        //                    System.IO.Directory.CreateDirectory(Server.MapPath(childDir));
        //                }
        //            }
        //            else
        //            {
        //                var stream = entry.Open();

        //                string finalPath = Path.Combine(Path.Combine(Server.MapPath(rootDir), fullpath));
        //                using (FileStream outputFileStream = new FileStream(finalPath, FileMode.Create))
        //                {
        //                    stream.CopyTo(outputFileStream);
        //                }
        //            }
        //            //var stream = entry.Open();

        //            //string newDir = "../Content/";

        //            //string finalPath = Path.Combine(Path.Combine(Server.MapPath(newDir), "test/file1"));
        //            //using (FileStream outputFileStream = new FileStream(finalPath, FileMode.Create))
        //            //{
        //            //    stream.CopyTo(outputFileStream);
        //            //}
        //        }
        //    }

        //    return RedirectToAction("UploadFile", "Admin", new { msg = "File Stored successfully", color = "green" });
        //}
        #endregion

    }
}