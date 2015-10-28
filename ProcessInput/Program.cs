using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessInput
{
    class Program
    {
        static void Main(string[] args)
        {
            String Wordlist = "";
            String processedFile = "";
            if(args.Length ==2)
            {
                Wordlist = args[0];
                processedFile = args[1];
            }
            else if (args.Length == 1)
            {
                Wordlist = args[0];
                processedFile = @"wordlist_ex_out";
            }
            else
            {
                Wordlist = @"C:\compling570\HW4\wordlist_ex";
                processedFile = @"C:\compling570\HW4\wordlist_ex_out";
            }
                
            String line;
            StreamWriter Outfile = new StreamWriter(processedFile);
            using (StreamReader file  =new StreamReader (Wordlist))
            {
                while((line = file.ReadLine())!= null && !String.IsNullOrWhiteSpace(line))
                {
                    char[] arr = line.ToCharArray();
                    foreach (var item in arr)
                    {
                        Outfile.Write("\"" + item.ToString() + "\" ");
                    }
                    Outfile.WriteLine();
                }
            }
            Outfile.Close();
            //Console.ReadLine();
        }
    }
}
