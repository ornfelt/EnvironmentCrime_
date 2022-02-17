using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EnvironmentCrime.Models
{
    public class EFEnvironmentCrimeRepository : IEnvironmentRepository //inherits from IEnvironmentRepository
    {
        private ApplicationDbContext context;

        //constructor
        public EFEnvironmentCrimeRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        //Create Errand
        public string SaveErrand(Errand errand)
        {
            if (errand.ErrandID == 0)
            {
                var currentVal = context.Sequences.Where(cv => cv.Id == 1).First(); //get CurrentValue for id = 1
                errand.RefNumber = "2018-45-" + currentVal.CurrentValue.ToString();
                errand.StatusId = "S_A";
                //errand.EmployeeId = "";
                //errand.DepartmentId = "";
                context.Errands.Add(errand);
                context.SaveChanges();
                UpdateSequenceValue(currentVal); //calls the method that adds 1 to currentvalue
            }
            else
            {
                Errand dbEntry = context.Errands.FirstOrDefault(e => e.ErrandID == errand.ErrandID);
                if (dbEntry != null)
                {
                    dbEntry.Place = errand.Place;
                    dbEntry.Observation = errand.Observation;
                    context.SaveChanges();
                }
            }

            string refNumb = errand.RefNumber;
            return refNumb;
        }

        //Read Errand
        public IQueryable<Errand> Errands => context.Errands.Include(e => e.Samples).Include(e => e.Pictures); //gets Errands from dbContext with samples and pictures included

        public Task<Errand> GetErrandDetail(int id)
        {
            return Task.Run(() =>
            {
                var errandDetail = context.Errands.Where(ed => ed.ErrandID == id).FirstOrDefault(); //changed to look at context.Errands instead of just Errands.Where...
                return (errandDetail);
            });
        }

        public Errand GetErrandInfo(int id)
        {
            var errandDetail = context.Errands.Where(ed => ed.ErrandID == id).FirstOrDefault(); //changed to look at context.Errands instead of just Errands.Where...

            return (errandDetail);
        }

        //Delete Errand
        public Errand DeleteErrand(int id)
        {
            Errand dbEntry = context.Errands.FirstOrDefault(e => e.ErrandID == id);
            if (dbEntry != null)
            {
                context.Errands.Remove(dbEntry);
            }
            context.SaveChanges();
            return dbEntry;
        }

        //Update Sequence
        public void UpdateSequenceValue(Sequence sequence)
        {
            Sequence dbEntry = context.Sequences.FirstOrDefault(s => s.Id == 1);
            if (dbEntry != null)
            {
                dbEntry.CurrentValue = (sequence.CurrentValue + 1);
                context.SaveChanges();
            }
        }

        //Update ErrandStatus
        public void UpdateErrandStatus(int id, string status)
        {

            Errand dbEntry = context.Errands.FirstOrDefault(e => e.ErrandID == id);
            if (dbEntry != null)
            {
                dbEntry.StatusId = status;
            }
            context.SaveChanges();
        }

        public void UpdateErrandDepartment(int id, string department)
        {
            Errand dbEntry = context.Errands.FirstOrDefault(e => e.ErrandID == id);
            if (dbEntry != null)
            {
                dbEntry.DepartmentId = department;
                //var errandDetail = context.Errands.Where(ed => ed.ErrandID == id).FirstOrDefault();
                try
                {
                    var employeeDepartment = context.Employees.Where(em => em.EmployeeId == dbEntry.EmployeeId).FirstOrDefault().DepartmentId; //gets DepartmentId from the errands employee
                    if (employeeDepartment != department)
                    {
                        dbEntry.EmployeeId = null;
                    }
                }
                catch(Exception e)
                {

                }
            }
            context.SaveChanges();
        }

        public void UpdateInvestigator(int id, string investigator, string reason, bool noAction)
        {
            Errand dbEntry = context.Errands.FirstOrDefault(e => e.ErrandID == id);

            if (dbEntry != null)
            {
                if (noAction)
                {
                    dbEntry.StatusId = "S_B";
                    dbEntry.EmployeeId = null;
                    if (reason.Equals("Ange motivering"))
                    {
                        reason = "";
                    }
                    dbEntry.InvestigatorInfo = reason;
                }
                else
                {
                    dbEntry.EmployeeId = investigator;
                }
            }
            context.SaveChanges();
        }

        public void UpdateInvestigatorAction(int id, string action)
        {
            Errand dbEntry = context.Errands.FirstOrDefault(e => e.ErrandID == id);
            if (dbEntry.InvestigatorAction == "") //if info is already empty only info is added, else with space between previous info
            {
                dbEntry.InvestigatorAction = action;
            }
            else
            {
                dbEntry.InvestigatorAction += (" " + action);
            }
            context.SaveChanges();
        }

        public void UpdateInvestigatorInfo(int id, string info)
        {
            Errand dbEntry = context.Errands.FirstOrDefault(e => e.ErrandID == id);
            if (dbEntry != null)
            {
                if (dbEntry.InvestigatorInfo == "") //if info is already empty only info is added, else with space between previous info
                {
                    dbEntry.InvestigatorInfo = info;
                }
                else
                {
                    dbEntry.InvestigatorInfo += (" " + info);
                }
            }
            context.SaveChanges();
        }

        public void UpdatePicture(int id, string picturePath)
        {
            //update in picture table
            context.Pictures.Add(new Picture { PictureName = picturePath, ErrandId = id });
            context.SaveChanges();
        }

        public void UpdateSample(int id, string samplePath)
        {
            //update in sample table
            context.Samples.Add(new Sample { SampleName = samplePath, ErrandId = id });
            context.SaveChanges();
        }

        public string PicturePath(int id)
        {
      try
      {
        Picture dbEntry = context.Pictures.FirstOrDefault(p => p.ErrandId == id); //gets last picture uploaded, can be switched to FirstOrDefault
        if (dbEntry != null)
        {
          return dbEntry.PictureName;
        }
        else
        {
          return "";
        }
      }
      catch(Exception e)
      {
        Console.WriteLine("The process failed: {0}", e.ToString());
        return "";
      }
        }

        public string SamplePath(int id)
        {
            Sample dbEntry = context.Samples.FirstOrDefault(s => s.ErrandId == id); //gets last sample uploaded, can be switched to FirstOrDefault
            if (dbEntry != null)
            {
                return dbEntry.SampleName;
            }
            else
            {
                return "";
            }
        }

        /*
         * Delete methods for removing tests..
         */
        public Sample DeleteSample(int id)
        {
            Sample dbEntry = context.Samples.FirstOrDefault(s => s.SampleId == id);
            if (dbEntry != null)
            {
                context.Samples.Remove(dbEntry);
            }
            context.SaveChanges();
            return dbEntry;
        }

        public Picture DeletePicture(int id)
        {
            Picture dbEntry = context.Pictures.FirstOrDefault(p => p.PictureId == id);
            if (dbEntry != null)
            {
                context.Pictures.Remove(dbEntry);
            }
            context.SaveChanges();
            return dbEntry;
        }

        public IQueryable<Department> Departments => context.Departments;
        public IQueryable<ErrandStatus> ErrandStatuses => context.ErrandStatuses;
        public IQueryable<Employee> Employees => context.Employees;
    }
}
