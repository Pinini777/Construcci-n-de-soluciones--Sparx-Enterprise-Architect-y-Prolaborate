using System;
using System.Windows.Forms;

namespace Addino
{
    public class AddinoClass
    {
        private const string MenuHeader = "-&Addino";
        private const string MenuHello = "&Say Hello";
        private const string MenuGoodbye = "&Say Goodbye";


        public string EA_Connect(EA.Repository repository)
        {
            return "Addino conectado correctamente";
        }


        public object EA_GetMenuItems(
            EA.Repository repository,
            string location,
            string menuName)
        {
            switch (menuName)
            {
                case "":
                    return MenuHeader;

                case MenuHeader:
                    return new string[]
                    {
                        MenuHello,
                        MenuGoodbye
                    };
            }

            return "";
        }


        public void EA_GetMenuState(
            EA.Repository repository,
            string location,
            string menuName,
            string itemName,
            ref bool isEnabled,
            ref bool isChecked)
        {
            isEnabled = true;
        }


        public void EA_MenuClick(
            EA.Repository repository,
            string location,
            string menuName,
            string itemName)
        {
            switch (itemName)
            {
                case MenuHello:

                    MessageBox.Show(
                        "Hola desde mi primer Add-in de Enterprise Architect",
                        "Addino");

                    break;


                case MenuGoodbye:

                    MessageBox.Show(
                        "Cerrando Addino",
                        "Addino");

                    break;
            }
        }


        public void EA_Disconnect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}