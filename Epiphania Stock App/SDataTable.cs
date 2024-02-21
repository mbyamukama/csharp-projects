using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows;
using FirebirdSql.Data.FirebirdClient;

namespace StockApp
{
    public class SDataTable: DataTable
    {
        public FbDataAdapter Adapter
        {
            get;
            set;
        }

        public int UpdateSource()
        {
            int affRows = 0;
            try
            {
                if (this.Adapter == null) throw new Exception("The Adapter property of this DataTable has not been initialised");
                else
                {
                    FbCommandBuilder cmdb = new FbCommandBuilder(this.Adapter);
                    {
                        affRows = this.Adapter.Update(this);
                    }
                }
            }
            catch (FbException ex)
            {
                System.Windows.MessageBox.Show("An error occurred.\n" + ex.Message);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("An error occurred.\n" + ex.Message);
            }
            return affRows;
        }
    }
}
