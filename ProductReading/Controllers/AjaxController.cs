using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProductReading.Models;
using ProductReading.BL;
using ProductReading.Helping_Classes;

namespace ProductReading.Controllers
{
    public class AjaxController : Controller
    {
        public GeneralPurpose gp = new GeneralPurpose();
        public DatabaseEntities de = new DatabaseEntities();

        #region Admin Controller

        [HttpPost]
        public ActionResult GetProductDataTableList(int isNew = -1, string productSku = "", string productType = "", string productName = "")
        {
            List<ProductRecord> plist = new ProductRecordBL().GetActiveProductsList(de).OrderByDescending(x => x.CreatedAt).ToList();

            if (isNew != -1)
            {
                if(isNew == 1)
                {
                    plist = plist.Where(x => x.IsNew == isNew).ToList();
                }
                else
                {
                    plist = plist.Where(x => x.IsNew == 0 || x.IsNew == null).ToList();
                }
            }
            if (productSku != "")
            {
                plist = plist.Where(x => x.Product_SKU.ToLower().Contains(productSku.ToLower())).ToList();
            }
            if (productType != "")
            {
                plist = plist.Where(x => x.Product_Type.ToLower().Contains(productType.ToLower())).ToList();
            }
            if (productName != "")
            {
                plist = plist.Where(x => x.Product_Name.ToLower().Contains(productName.ToLower())).ToList();
            }

            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];

            if (sortColumnName != "" && sortColumnName != null)
            {
                if (sortColumnName != "0")
                {
                    if (sortDirection == "asc")
                    {
                        plist = plist.OrderByDescending(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                    }
                    else
                    {
                        plist = plist.OrderBy(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                    }
                }
            }

            int totalrows = plist.Count();

            //filter
            if (!string.IsNullOrEmpty(searchValue))
            {
                plist = plist.Where(x => x.Product_Group != null && x.Product_Group.ToLower().Contains(searchValue.ToLower()) ||
                                    x.Manufacturer != null && x.Manufacturer.ToLower().Contains(searchValue.ToLower()) ||
                                    x.Short_Description != null && x.Short_Description.ToLower().Contains(searchValue.ToLower()) ||
                                    x.Long_Description != null && x.MAP_Price.ToLower().Contains(searchValue.ToLower()) ||
                                    x.MAP_Price != null && x.MAP_Price.ToLower().Contains(searchValue.ToLower()) ||
                                    x.Kroll_Sell != null && x.Kroll_Sell.ToLower().Contains(searchValue.ToLower())
                                    ).ToList();
            }

            int totalrowsafterfilterinig = plist.Count();


            // pagination
            plist = plist.Skip(start).Take(length).ToList();

            List<ProductRecordDTO> pdto = new List<ProductRecordDTO>();

            foreach (ProductRecord p in plist)
            {
                ProductRecordDTO obj = new ProductRecordDTO()
                {
                    Id = p.Id,
                    EncId = StringCipher.EncryptId(p.Id),
                    IsNew = p.IsNew,
                    ProductImagePath = p.ProductImagePath,
                    Vendor = "Kroll",
                    Product_SKU = p.Product_SKU,
                    Product_Group = p.Product_Group,
                    Manufacturer = p.Manufacturer,
                    Product_Type = p.Product_Type,
                    Product_Name = p.Product_Name,
                    Short_Description = p.Short_Description,
                    Long_Description = p.Long_Description,
                    Level_1_Category = p.Level_1_Category,
                    Level_2_Category = p.Level_2_Category,
                    UPC_Code = p.UPC_Code,
                    MAP_Price = p.MAP_Price,
                    Kroll_Sell = p.Kroll_Sell,
                    MSRP = p.MSRP,
                    Product_Weight = p.Product_Weight,
                    Small_Image_File = p.Small_Image_File,
                    Small_Image_Text = p.Small_Image_Text,
                    Manu_Part_Number = p.Manu_Part_Number,
                    Hazardous = p.Hazardous,
                    Restricted = p.Restricted,
                    Complete_SKU = p.Complete_SKU,
                    SKU_Variation = p.SKU_Variation,
                    Parent_SKU = p.Parent_SKU,
                    Choice1 = p.Choice1,
                    Option1 = p.Option1,
                    Choice2 = p.Choice2,
                    Option2 = p.Option2,
                    Choice3 = p.Choice3,
                    Option3 = p.Option3,
                    Choice4 = p.Choice4,
                    Option4 = p.Option4,
                    Choice5 = p.Choice5,
                    Option5 = p.Option5,
                    Choice6 = p.Choice6,
                    Option6 = p.Option6,
                    Choice7 = p.Choice7,
                    Option7 = p.Option7,
                    Choice8 = p.Choice8,
                    Option8 = p.Option8,
                    Choice9 = p.Choice9,
                    Option9 = p.Option9,
                    Choice10 = p.Choice10,
                    Option10 = p.Option10,
                    QTY_Available = p.QTY_Available,
                    Country_of_Origin_Code = p.Country_of_Origin_Code,
                    CreatedAt = Convert.ToDateTime(p.CreatedAt).ToString("MM/dd/yyyy")
                };

                pdto.Add(obj);
            }

            return Json(new { data = pdto, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult GetProductById(int id)
        {
            ProductRecord p = new ProductRecordBL().GetActiveProductById(id, de);
            
            if (p == null)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

            ProductRecordDTO obj = new ProductRecordDTO()
            {
                Id = p.Id,
                EncId = StringCipher.EncryptId(p.Id),
                Product_SKU = p.Product_SKU,
                Product_Group = p.Product_Group,
                Manufacturer = p.Manufacturer,
                Product_Type = p.Product_Type,
                Product_Name = p.Product_Name,
                Short_Description = p.Short_Description,
                Long_Description = p.Long_Description,
                Level_1_Category = p.Level_1_Category,
                Level_2_Category = p.Level_2_Category,
                UPC_Code = p.UPC_Code,
                MAP_Price = p.MAP_Price,
                Kroll_Sell = p.Kroll_Sell,
                MSRP = p.MSRP,
                Product_Weight = p.Product_Weight,
                Small_Image_File = p.Small_Image_File,
                Small_Image_Text = p.Small_Image_Text,
                Manu_Part_Number = p.Manu_Part_Number,
                Hazardous = p.Hazardous,
                Restricted = p.Restricted,
                Complete_SKU = p.Complete_SKU,
                SKU_Variation = p.SKU_Variation,
                Parent_SKU = p.Parent_SKU,
                Choice1 = p.Choice1,
                Option1 = p.Option1,
                Choice2 = p.Choice2,
                Option2 = p.Option2,
                Choice3 = p.Choice3,
                Option3 = p.Option3,
                Choice4 = p.Choice4,
                Option4 = p.Option4,
                Choice5 = p.Choice5,
                Option5 = p.Option5,
                Choice6 = p.Choice6,
                Option6 = p.Option6,
                Choice7 = p.Choice7,
                Option7 = p.Option7,
                Choice8 = p.Choice8,
                Option8 = p.Option8,
                Choice9 = p.Choice9,
                Option9 = p.Option9,
                Choice10 = p.Choice10,
                Option10 = p.Option10,
                QTY_Available = p.QTY_Available,
                Country_of_Origin_Code = p.Country_of_Origin_Code,
            };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        #endregion

        [HttpPost]
        public ActionResult ValidateEmail(string email, int id = -1)
        {
            return Json(gp.ValidateEmail(email, id), JsonRequestBehavior.AllowGet);
        }
    }
}