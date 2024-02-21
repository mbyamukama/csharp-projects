using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using FirebirdSql.Data.FirebirdClient;

namespace FRESUgandaERP
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
            if (this.Adapter == null) throw new Exception("The Adapter property of this DataTable has not been initialised");
            else
            {
                FbCommandBuilder cmdb = new FbCommandBuilder(this.Adapter);
                affRows = this.Adapter.Update(this);
            }
            return affRows;
        }
    }
}
