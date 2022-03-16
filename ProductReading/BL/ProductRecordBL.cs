using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProductReading.Models;
using ProductReading.DAL;

namespace ProductReading.BL
{
    public class ProductRecordBL
    {
        public List<ProductRecord> GetAllProductsList(DatabaseEntities de)
        {
            return new ProductRecordDAL().GetAllProductsList(de);
        }

        public List<ProductRecord> GetActiveProductsList(DatabaseEntities de)
        {
            return new ProductRecordDAL().GetActiveProductsList(de);
        }

        public ProductRecord GetProductById(int id, DatabaseEntities de)
        {
            return new ProductRecordDAL().GetProductById(id, de);
        }

        public ProductRecord GetActiveProductById(int id, DatabaseEntities de)
        {
            return new ProductRecordDAL().GetActiveProductById(id, de);
        }

        public ProductRecord GetActiveProductBySearch(string sku, string price, DatabaseEntities de)
        {
            return new ProductRecordDAL().GetActiveProductBySearch(sku, price, de);
        }

        public ProductRecord GetActiveProductBySku(string sku, DatabaseEntities de)
        {
            return new ProductRecordDAL().GetActiveProductBySku(sku, de);
        }

        public bool UpdateProductQuantity(ProductRecord Product, DatabaseEntities de)
        {
            return new ProductRecordDAL().UpdateProductQuantity(Product, de);
        }

        public List<ProductRecord> GetNewProducts(DatabaseEntities de)
        {
            return new ProductRecordDAL().GetNewProducts(de);
        }

        public bool ClearNewProduct(ProductRecord Product, DatabaseEntities de)
        {
            return new ProductRecordDAL().ClearNewProduct(Product, de);
        }

        public bool AddProduct(ProductRecord Product, DatabaseEntities de)
        {
            return new ProductRecordDAL().AddProduct(Product, de);
        }


        public int AddProduct2(ProductRecord Product, DatabaseEntities de)
        {
            return new ProductRecordDAL().AddProduct2(Product, de);
        }

        public bool UpdateProduct(ProductRecord Product, DatabaseEntities de)
        {
            return new ProductRecordDAL().UpdateProduct(Product, de);
        }

        public bool DeleteProduct(int id, DatabaseEntities de)
        {
            return new ProductRecordDAL().DeleteProduct(id, de);
        }
        
        //public bool ClearNewProductTag(DatabaseEntities de)
        //{
        //    return new ProductRecordDAL().ClearNewProductTag(de);
        //}


        #region CommonTable

        public CommonTable GetLastRefresh(DatabaseEntities de)
        {
            return new ProductRecordDAL().GetLastRefresh(de);
        }

        public bool AddLastRefresh(CommonTable commonTable, DatabaseEntities de)
        {
            return new ProductRecordDAL().AddLastRefresh(commonTable, de);
        }

        public bool UpdateLastRefresh(CommonTable commonTable, DatabaseEntities de)
        {
            return new ProductRecordDAL().UpdateLastRefresh(commonTable, de);
        }

        #endregion

    }
}