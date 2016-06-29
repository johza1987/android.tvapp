using Assessment.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Assessment.Class
{
    public class Common
    {
        public List<SchoolModel> GetSchool()
        {          
            string path = HttpContext.Current.Server.MapPath("~/App_Data/DataSchool.txt");
            string data = System.IO.File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<SchoolModel>>(data);
        }
        public List<SchoolGroupModel> GetSchoolGroup()
        {
            string path = HttpContext.Current.Server.MapPath("~/App_Data/DataSchoolGroup.txt");
            string data = System.IO.File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<SchoolGroupModel>>(data);
        }
        public List<StudentModel> GetStudent()
        {
            string path = HttpContext.Current.Server.MapPath("~/App_Data/DataStudent.txt");
            string data = System.IO.File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<StudentModel>>(data);
        }
        public List<DisabledTypeModel> GetDisabledType()
        {
            string path = HttpContext.Current.Server.MapPath("~/App_Data/DataDisabledType.txt");
            string data = System.IO.File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<DisabledTypeModel>>(data);
        }
        public List<StudentDetailModel> GetStudentDetail()
        {
            string path = HttpContext.Current.Server.MapPath("~/App_Data/DataStudentDetail.txt");
            string data = System.IO.File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<StudentDetailModel>>(data);
        }
        public List<SchoolTypeModel> GetSchoolType()
        {
            string path = HttpContext.Current.Server.MapPath("~/App_Data/DataSchoolType.txt");
            string data = System.IO.File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<SchoolTypeModel>>(data);
        }
        public List<AssessmentModel> GetAssessment()
        {
            string path = HttpContext.Current.Server.MapPath("~/App_Data/DataAssessment.txt");
            string data = System.IO.File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<AssessmentModel>>(data);
        }
        public List<AssessDetailModel> GetAssessDetail()
        {
            string path = HttpContext.Current.Server.MapPath("~/App_Data/DataAssessDetail.txt");
            string data = System.IO.File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<AssessDetailModel>>(data);
        }
        public List<AssessTypeModel> GetAssessType()
        {
            string path = HttpContext.Current.Server.MapPath("~/App_Data/DataAssessType.txt");
            string data = System.IO.File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<AssessTypeModel>>(data);
        }
    }
}