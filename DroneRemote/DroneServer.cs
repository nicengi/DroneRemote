using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneRemote
{
    class DroneServer
    {

        #region Fields

        #endregion

        #region Constructor
        public DroneServer()
        {
            HttpClient = new HttpClient();
            ServerUrl = "http://101.43.217.188:9200/code01/";
        }
        #endregion

        #region Properties
        public HttpClient HttpClient { get; }
   
        public string ServerUrl { get; set; }
        #endregion

        #region Methods
        #endregion
    }
}
