using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnvironmentCrime.Models
{
  //repository interface
    public interface IEnvironmentRepository //changed to public
    {
        
        IQueryable<Department> Departments { get; }
        IQueryable<Employee> Employees { get; }
        IQueryable<ErrandStatus> ErrandStatuses { get; }

        //Create
        string SaveErrand(Errand errand);

        //Read
        IQueryable<Errand> Errands { get; }
        Task<Errand> GetErrandDetail(int id);
        Errand GetErrandInfo(int id);
        string PicturePath(int id);
        string SamplePath(int id);

        //Delete
        Errand DeleteErrand(int id);
        Sample DeleteSample(int id);
        Picture DeletePicture(int id);

        //Update
        void UpdateSequenceValue(Sequence sequence);
        void UpdateErrandStatus(int id, string status);
        void UpdateErrandDepartment(int id, string department);
        void UpdateInvestigator(int id, string investigator, string reason, bool noAction);
        void UpdateInvestigatorAction(int id, string action);
        void UpdateInvestigatorInfo(int id, string info);
        void UpdatePicture(int id, string picturePath);
        void UpdateSample(int id, string samplePath);
    }
}
