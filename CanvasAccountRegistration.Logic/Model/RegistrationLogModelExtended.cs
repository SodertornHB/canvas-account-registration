using Newtonsoft.Json;

namespace CanvasAccountRegistration.Logic.Model
{
    public partial class RegistrationLog 
    {
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
} 