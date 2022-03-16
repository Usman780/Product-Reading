using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProductReading.Models;

namespace ProductReading.DAL
{
    public class ProductRecordDAL
    {
        public List<ProductRecord> GetAllProductsList(DatabaseEntities de)
        {
            return de.ProductRecords.ToList();
        }

        public List<ProductRecord> GetActiveProductsList(DatabaseEntities de)
        {
            //return de.ProductRecords.Where(x => x.IsActive == 1).ToList();
            return de.sp_GetActiveProductList().ToList();
        }

        public ProductRecord GetProductById(int id, DatabaseEntities de)
        {
            return de.ProductRecords.Where(x => x.Id == id).FirstOrDefault();
        }

        public ProductRecord GetActiveProductById(int id, DatabaseEntities de)
        {
            //return de.ProductRecords.Where(x => x.Id == id).FirstOrDefault(x => x.IsActive == 1);
            return de.sp_GetActiveProductById(id).FirstOrDefault();
        }

        public ProductRecord GetActiveProductBySearch(string sku, string price, DatabaseEntities de)
        {
            //return de.ProductRecords.Where(x => x.Product_SKU.ToLower() == sku.ToLower() && x.MAP_Price.ToLower() == price.ToLower()).FirstOrDefault(x => x.IsActive == 1);
            return de.sp_GetProductBySearch(sku.ToLower(), price.ToLower()).FirstOrDefault();
        }


        public ProductRecord GetActiveProductBySku(string sku, DatabaseEntities de)
        {
            //return de.ProductRecords.Where(x => x.Product_SKU.ToLower() == sku.ToLower()).FirstOrDefault(x => x.IsActive == 1);
            return de.sp_GetActiveProductBySku(sku.ToLower()).FirstOrDefault();
        }

        public bool UpdateProductQuantity(ProductRecord Product, DatabaseEntities de)
        {
            try
            {
                de.Entry(Product).State = System.Data.Entity.EntityState.Modified;
                de.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<ProductRecord> GetNewProducts(DatabaseEntities de)
        {
            //return de.ProductRecords.Where(x => x.IsNew == 1 && x.IsActive == 1).ToList();
            return de.sp_GetNewProducts().ToList();
        }

        public bool ClearNewProduct(ProductRecord Product, DatabaseEntities de)
        {
            try
            {
                Product.IsNew = 0;

                return UpdateProduct(Product, de);
            }
            catch
            {
                return false;
            }
        }

        public bool AddProduct(ProductRecord Product, DatabaseEntities de)
        {
            try
            {
                de.ProductRecords.Add(Product);
                de.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        

        public int AddProduct2(ProductRecord Product, DatabaseEntities de)
        {
            try
            {
                de.ProductRecords.Add(Product);
                de.SaveChanges();

                return Product.Id;
            }
            catch
            {
                return -1;
            }
        }

        public bool UpdateProduct(ProductRecord Product, DatabaseEntities de)
        {
            try
            {
                de.Entry(Product).State = System.Data.Entity.EntityState.Modified;
                de.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteProduct(int id, DatabaseEntities de)
        {
            try
            {
                de.ProductRecords.Remove(de.ProductRecords.Where(x => x.Id == id).FirstOrDefault());
                de.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }


        //public bool ClearNewProductTag(DatabaseEntities de)
        //{
        //    try
        //    {
        //        using (de)
        //        {
        //            de.ProductRecords.Where(x => x.IsNew == 1 && x.IsActive == 1).ToList().ForEach(x => x.IsNew = 0);
        //            de.SaveChanges();
        //        }
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}


        #region CommonTable
        public CommonTable GetLastRefresh(DatabaseEntities de)
        {
            return de.CommonTables.Where(x => x.IsActive == 1).FirstOrDefault();
        }

        public bool AddLastRefresh(CommonTable commonTable, DatabaseEntities de)
        {
            try
            {
                de.CommonTables.Add(commonTable);
                de.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateLastRefresh(CommonTable commonTable, DatabaseEntities de)
        {
            try
            {
                de.Entry(commonTable).State = System.Data.Entity.EntityState.Modified;
                de.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

    }
}