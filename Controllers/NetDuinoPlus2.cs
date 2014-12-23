namespace HomeAutomation
{
    public class NetDuinoPlus2 : IController
    {

        private readonly I2CBus _twiBus;



        public I2CBus TwiBus
        {
            get { return I2CBus.GetInstance(); }
        }

        public NetDuinoPlus2()
        {

        }

        public void AddComponent(IComponent component)
        {

        }
    }
}