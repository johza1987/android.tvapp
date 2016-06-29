using SinetWifi.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SinetWifi.Models
{
    [Table("generate_code")]
    public class GenerateCode
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string prefix { get; set; }
        public DateTime last_gendate { get; set; }
        public int last_number { get; set; }
        public int digit_number { get; set; }

        public static string GetUserinfoCodeRunning()
        {
            int DocID = 1;

            string docrunning = string.Empty;

            using (DatabaseContext db = new DatabaseContext())
            {
                var doc = db.GenerateCode.Find(DocID);

                if (doc != null)
                {
                    int run = doc.last_number;

                    if (doc.last_gendate == null)
                    {
                        doc.last_gendate = DateTime.Now;
                    }
                    else
                    {
                        if (DateTime.Now.Year != ((DateTime)doc.last_gendate).Year)
                        {
                            doc.last_gendate = DateTime.Now;
                            run = 0;
                        }
                    }

                    run++;
                    docrunning = doc.prefix.Trim() + doc.last_gendate.ToString("yy", Shared.CultureInfo) + run.ToString().PadLeft(doc.digit_number, '0');
                    doc.last_number = run;
                    db.Entry(doc).State = EntityState.Modified;
                }
            }

            return docrunning;
        }
    }
}