using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using FirebirdSql.Data.Isql;

namespace TestFirebirdBlobs
{
    class Program
    {
        static void Main(string[] args)
        {           
            Console.WriteLine("Starting Against Test DB: " + args[0] + " " + args[1] + " " + args[2]);
            Console.WriteLine("Enter to Start");
            Console.ReadLine();

            FbConnectionStringBuilder csb = new FbConnectionStringBuilder();
            csb.DataSource = "localhost";
            csb.Port = 3050;
            csb.UserID = args[1];
            csb.Password = args[2];
            csb.Database = args[0];
            csb.ServerType = FbServerType.Default;
            Console.WriteLine("Dropping Database");
            FbConnection.DropDatabase(csb.ToString());
            Console.WriteLine("Creating Database");
            FbConnection.CreateDatabase(csb.ToString());

            FileInfo Schema = new FileInfo("schema.sql");
            String DDL = Schema.OpenText().ReadToEnd();
            Console.WriteLine("Applying Schema");
            CreateDatabaseSchema(DDL, csb.ToString());

            try
            {
                using (FbConnection db = new FbConnection(csb.ToString()))
                {
                    db.Open();
                    List<int> loops = Enumerable.Range(1, 3).ToList();
                    foreach (int k in loops)
                    {
                        using (FbTransaction t = db.BeginTransaction())
                        {
                            try
                            {
                                List<int> words = Enumerable.Range(1, 1).ToList();
                                foreach (int i in words)
                                {
                                    MemoDB m = new MemoDB();
                                    m.MEMO = "TEST";
                                    db.Insert(m, t);
                                }
                                if (k == 2)
                                {
                                    Console.WriteLine("Simulating a Close");
                                    db.Close();
                                }
                                else
                                {
                                    t.Commit();
                                    var count = db.Query<Int32>("select count(*) from memo").Single();
                                    Console.WriteLine("Records Written: " + count);
                                }
                            }
                            catch (Exception ex)
                            {
                                t.Rollback();
                                WriteExceptionLog(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteExceptionLog(ex);
            }
            Console.WriteLine("Stopped... Press Enter to Check for Validation.");
            Console.ReadLine();
            try
            {
                using (FbConnection db = new FbConnection(csb.ToString()))
                {
                    db.Open();
                    Console.WriteLine("Counting");
                    var count = db.Query<Int32>("select count(*) from memo").Single();
                    Console.WriteLine("Records Written: " + count);
                    Console.WriteLine("Deleting");
                    db.Execute("delete from memo");
                }
            }
            catch (Exception ex)
            {
                WriteExceptionLog(ex);
            }
            Console.WriteLine("Ending Application.");
            Console.ReadLine();          
        }

        private static void CreateDatabaseSchema(String Schema, string CSB)
        {
            using (FbConnection db = new FbConnection(CSB))
            {
                FbScript fbs = new FbScript(Schema);
                fbs.Parse();

                FbBatchExecution fbe = new FbBatchExecution(db, fbs);
                fbe.Execute(true);
            }
        }

        private static void WriteExceptionLog(Exception ex)
        {
            Console.WriteLine(ex.Message);
            //Console.WriteLine(ex.StackTrace);
            File.AppendAllText("exlog.txt", ex.Message + Environment.NewLine);
            //File.AppendAllText("exlog.txt", ex.StackTrace + Environment.NewLine);
        }
    }
}
