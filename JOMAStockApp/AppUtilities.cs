using System.Data;

namespace StockApp
{
    public static class GlobalSystemData
    {
        public static Session Session = null;
        public static DataTable Settings = null;
        public static int RoundUpToNearestNote(double number)
        {
            int temp = (int)number;
            while (temp % 100 != 0)
            {
                ++temp;
            }
            return temp;
        }
        public enum SaleViewType {All=0, AllShowingVAT, SingleCustomerWithTIN,TinCustomersOnly};
        public static DataTable addStockdataTable = null;
        public static void InitStockDataTable()
        {
            addStockdataTable = new DataTable();
            addStockdataTable.Columns.Add("ENTRYID");
            addStockdataTable.Columns.Add("BARCODE");
            addStockdataTable.Columns.Add("ITEMNAME");
            addStockdataTable.Columns.Add("SUPPLIER");
            addStockdataTable.Columns.Add("INVOICENUM");
            addStockdataTable.Columns.Add("QUANTITY");
            addStockdataTable.Columns.Add("COSTPPU");
            addStockdataTable.Columns.Add("COSTTOTAL"); 
            addStockdataTable.Columns["COSTTOTAL"].DataType = typeof(int);
            addStockdataTable.Columns.Add("FACTOR");
            addStockdataTable.Columns.Add("CATEGORY");
            addStockdataTable.Columns.Add("DATEOFENTRY");
        } 
        
      //EFRIS device information
        
       
    }
}
