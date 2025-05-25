using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KompilatoriLaba1
{
    using System;
    using System.Windows.Forms;

    namespace KompilatoriLaba1
    {
        static class Program
        {
            [STAThread]
            static void Main()
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form2());
            }
        }
    }
    //internal static class Program
    //{
    //    /// <summary>
    //    /// Главная точка входа для приложения.
    //    /// </summary>
    //    [STAThread]
    //    static void Main()
    //    {
    //        Application.EnableVisualStyles();
    //        Application.SetCompatibleTextRenderingDefault(false);
    //        Application.Run(new Compiler());
    //    }
    //}
}
