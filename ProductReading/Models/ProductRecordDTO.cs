using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductReading.Models
{
    public class ProductRecordDTO
    {
        public int Id { get; set; }
        public string EncId { get; set; }
        public Nullable<int> IsNew { get; set; }
        public string ProductImagePath { get; set; }
        public string Vendor { get; set; }
        public string Product_SKU { get; set; }
        public string Product_Group { get; set; }
        public string Manufacturer { get; set; }
        public string Product_Type { get; set; }
        public string Product_Name { get; set; }
        public string Short_Description { get; set; }
        public string Long_Description { get; set; }
        public string Level_1_Category { get; set; }
        public string Level_2_Category { get; set; }
        public string UPC_Code { get; set; }
        public string MAP_Price { get; set; }
        public string Kroll_Sell { get; set; }
        public string MSRP { get; set; }
        public string Product_Weight { get; set; }
        public string Small_Image_File { get; set; }
        public string Small_Image_Text { get; set; }
        public string Manu_Part_Number { get; set; }
        public string Hazardous { get; set; }
        public string Restricted { get; set; }
        public string Complete_SKU { get; set; }
        public string SKU_Variation { get; set; }
        public string Parent_SKU { get; set; }
        public string Choice1 { get; set; }
        public string Option1 { get; set; }
        public string Choice2 { get; set; }
        public string Option2 { get; set; }
        public string Choice3 { get; set; }
        public string Option3 { get; set; }
        public string Choice4 { get; set; }
        public string Option4 { get; set; }
        public string Choice5 { get; set; }
        public string Option5 { get; set; }
        public string Choice6 { get; set; }
        public string Option6 { get; set; }
        public string Choice7 { get; set; }
        public string Option7 { get; set; }
        public string Choice8 { get; set; }
        public string Option8 { get; set; }
        public string Choice9 { get; set; }
        public string Option9 { get; set; }
        public string Choice10 { get; set; }
        public string Option10 { get; set; }
        public string QTY_Available { get; set; }
        public string Country_of_Origin_Code { get; set; }
        public string CreatedAt { get; set; }
    }
}