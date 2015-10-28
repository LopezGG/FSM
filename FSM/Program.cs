using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM
{
    class Program
    {
        static void Main(string[] args)
        {

            List<String> FinalStates = new List<string>();
            String lexiconPath;
            String FSAPath;
            String WordList_path="";
            if (args.Length > 1)
            {
                lexiconPath = args[1];
                FSAPath = args[0];
            }
            else if (args.Length == 1)
            {
                lexiconPath = FSAPath = args[0];
            }
            else
            {
                lexiconPath = @"C:\compling570\HW4\ex";
                FSAPath = @"C:\compling570\HW4\morph_rules_ex";
            }
            String Start = "";
            DataTable TransitionTable = GetTable(FinalStates, FSAPath, out Start);
            Dictionary<String, List<String>> lexicon = GetLexicon(lexiconPath);

            DataTable TransitionTableUpd = MergeLexiconTransition(TransitionTable, lexicon);

            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"output_fsm"))
            {
                file.WriteLine(FinalStates[0]);
                foreach (DataRow row in TransitionTableUpd.Rows)
                {
                    string term = row["Term"].ToString();
                    string replace = row["Replace"].ToString();
                    if (replace != "*e*")
                        replace = String.Format("\"" + replace + "\"");
                    if (!String.IsNullOrEmpty(term) && term != "*e*")
                        term = String.Format("\"" + term + "\"");

                    if (!String.IsNullOrEmpty(term))
                        file.WriteLine('(' + row["From"].ToString() + " (" + row["To"].ToString() + " " + replace + " " + term+ "))");
                    else
                        file.WriteLine('(' + row["From"].ToString() + " (" + row["To"].ToString() + " " + replace + " *e*))");
                }
            }
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"output_fsm2"))
            {
                file.WriteLine(FinalStates[0]);
                foreach (DataRow row in TransitionTableUpd.Rows)
                {
                    if (!String.IsNullOrEmpty(row["Term"].ToString()))
                        file.WriteLine('(' + row["From"].ToString() + " (" + row["To"].ToString() + " \"" + row["Replace"].ToString() +  "\"))");
                    else
                        file.WriteLine('(' + row["From"].ToString() + " (" + row["To"].ToString() + " \"" + row["Replace"].ToString() + "\"))");
                }
            }




            ////Print lexicon
            //foreach (KeyValuePair<String,List<String>> item in lexicon)
            //{
            //    Console.WriteLine("key : " + item.Key );
            //    item.Value.ForEach(Console.WriteLine);
            //    Console.WriteLine();
            //    Console.WriteLine();
            //}


            Console.ReadLine();
        }


        public static DataTable GetTable(List<String> FinalState, String FSAPath, out String Start)
        {
	        DataTable TransitionTable = new DataTable();
	        TransitionTable.Columns.Add(new DataColumn("From", typeof(string)));
	        TransitionTable.Columns.Add(new DataColumn("To", typeof(string)));
	        TransitionTable.Columns.Add(new DataColumn("Term", typeof(string)));
	        TransitionTable.Columns.Add(new DataColumn("Weight", typeof(double)));

	        string line;
	        int counter = 0;
	        Start = "";
	        // Read the file and display it line by line.
	        using (System.IO.StreamReader file =
	           new System.IO.StreamReader(FSAPath))
	        {
	           while ((line = file.ReadLine()) != null)
		        {
			        if (!String.IsNullOrWhiteSpace(line))
			        {
				        if (line.StartsWith("%"))
					        continue;
				        //get a list of fimal states
				        else if (counter == 0)
				        {
					        string[] words = line.Split(new char[] { ' ' });
					        foreach (string item in words)
					        {
						        FinalState.Add(item);
					        }
					        counter++;
					        continue;
				        }
				        else
				        {
					        string fromState = line.Substring(1, line.IndexOf(' ')-1);
					        if (counter ==1)
					        {
						        Start = fromState;
						        counter++;
					        }
					        int OpenParan = line.IndexOf('(', 1);
					        int CloseParan = line.IndexOf(')');
					        string data = line.Substring(OpenParan + 1, CloseParan - OpenParan - 1);
					        string[] dataWords = data.Split(new char[] { ' ' });
					        string toState;
					        string term;
					        double weight;
					        if (dataWords.Length == 3)
					        {
						        toState = dataWords[0];
						        term = dataWords[1];
						        weight = Convert.ToDouble(dataWords[2]);
					        }
					        else if (dataWords.Length == 2)
					        {
						        toState = dataWords[0];
						        term = dataWords[1];
						        weight = Convert.ToDouble(1.0);
					        }
					        else
					        {
						        throw new Exception("Incorrect Number of arguments");
					        }
					        TransitionTable.Rows.Add(fromState, toState, term, weight);
				        }
			        }
		        }
	        }
	        return TransitionTable;
        }
        


        //Given a lexicon and transition table generate a new table whic incorporates lexicon terms. The join is on the calss labels(eg regular_verb_stem)
        public static DataTable MergeLexiconTransition(DataTable TransitionTable, Dictionary<String, List<String>> lexicon)
        {
            string term;
            DataTable TransitionTableUpd = new DataTable();
            TransitionTableUpd.Columns.Add(new DataColumn("From", typeof(string)));
            TransitionTableUpd.Columns.Add(new DataColumn("To", typeof(string)));
            TransitionTableUpd.Columns.Add(new DataColumn("Term", typeof(string)));
            TransitionTableUpd.Columns.Add(new DataColumn("Replace", typeof(string)));
            TransitionTableUpd.Columns.Add(new DataColumn("Weight", typeof(double)));
            foreach (DataRow row in TransitionTable.Rows)
            {
                term = row["Term"].ToString();
                string from = row["From"].ToString();
                string to = row["To"].ToString();
                if (lexicon.ContainsKey(term))
                {
                    List<String> Stringlist = lexicon[term];
                    foreach (string item in Stringlist)
                    {
                        for (int i = 0; i < item.Length; i++)
                        {
                            string fnew = String.Format(from + "_" + (i - 1));
                            string tnew = String.Format(from + "_" + i);
                            string ModTerm = String.Format(item + "/" + term);
                            if (item.Length == 1)
                                TransitionTableUpd.Rows.Add(from, to, ModTerm, item[i], row["Weight"]);
                            else if (i == item.Length -1 )
                                TransitionTableUpd.Rows.Add(fnew, to, ModTerm, item[i], row["Weight"]);
                            else if (i == 0)
                                TransitionTableUpd.Rows.Add(from, tnew, "", item[i], row["Weight"]);
                            else
                                TransitionTableUpd.Rows.Add(fnew, tnew, "", item[i], row["Weight"]);
                        }
                    }
                }
                else if (term == "*e*")
                {
                    TransitionTableUpd.Rows.Add(row["From"], row["To"], term, term, row["Weight"]);
                }

            }
            return TransitionTableUpd;
        }
        //Convert a lexicon into a hashmap with class labels as key and words list as the value
        public static Dictionary<String, List<String>> GetLexicon(String PathToLexicon)
        {
            String key;
            String value;
            Dictionary<String, List<String>> lexicon = new Dictionary<String, List<String>>();
            String line;
            using (StreamReader reader = new StreamReader(@"C:\compling570\HW4\lexicon_ex"))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (!String.IsNullOrWhiteSpace(line))
                    {
                        String[] words = line.Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (words.Count() != 2)
                            throw new Exception("Incorrect format in the lexicon");
                        key = words[1];
                        value = words[0];
                        if (lexicon.ContainsKey(key))
                            lexicon[key].Add(value);
                        else
                        {
                            List<String> list = new List<string>();
                            list.Add(value);
                            lexicon.Add(key, list);
                        }
                    }
                }
            }
            return lexicon;
        }

        //reads through the FSA file and generates an datatable
    }    
}
