using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.HttpModel
{
    public class PostCanvasAccountRequestModel
    {
        public CanvasUserPostModel user { get; set; } = new CanvasUserPostModel();
        public PseudonymCanvasPostModel pseudonym { get; set; } = new PseudonymCanvasPostModel();
        public Communication_channelCanvasPostModel communication_channel { get; set; } = new Communication_channelCanvasPostModel();
        public bool Enable_sis_reactivation { get; set; } = true;
    }

    public class CanvasUserPostModel
    {
        public string Name { get; set; }
        public string Short_name { get; set; }
        public string Sortable_name { get; set; }
        public string Time_zone { get { return "Stockholm"; } }
        public string Locale { get; set; }
        public string Terms_of_use { get; set; }
        public bool Skip_registration { get { return true; } }
    }

    public class PseudonymCanvasPostModel
    {
        public string Unique_id { get; set; }
        //public string Password { get; set; }
        public string Sis_user_id { get; set; }
        public string Integration_id { get; set; }
        public bool Send_confirmation { get { return false; } }
        public bool Force_self_registration { get { return false; } }
        public string Authentication_provider_id { get; set; }
    }

    public class Communication_channelCanvasPostModel
    {
        public string Type { get { return "email"; } }
        public string Address { get; set; }
        public string Confirmation_url { get; set; }
        public bool Skip_confirmation { get { return true; } }
    }
    public class PostCanvasAccountResponseModel
    {
        public string id { get; set; }
    }
}
