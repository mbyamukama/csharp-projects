using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows;
using FirebirdSql.Data.FirebirdClient;

namespace HCOTIS
{
    [Serializable]
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
                if (ex.ErrorCode == 335544558)
                {
                    MessageBox.Show("An error occurred. The database refused to accept  a change on one of the rows. "+
                        "It may be that a constraint is being violated\nDETAILS: " + ex.Message, "ERROR");
                }
                else
                System.Windows.MessageBox.Show("An error occurred.\n" + ex.Message);
            }
            return affRows;
        }
    }
}
