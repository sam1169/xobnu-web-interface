using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xobnu.Domain.Abstract;
using Xobnu.Domain.Entities;

namespace Xobnu.Domain.Concrete
{
    public class EFCCGroupRepo : ICCGroupRepo
    {
        private EFDBContextClient context = new EFDBContextClient();

        public IQueryable<CCGroup> CCGroups
        {
            get { return context.CCGroups; }
            set { }
        }

        public CCGroup SaveGroup(CCGroup groupObj)
        {
            context.CCGroups.Add(groupObj);
            context.SaveChanges();
            return groupObj;
        }

        public bool DeleteGroup(long id)
        {
            CCGroup dbEntry = context.CCGroups.Find(id);
            if (dbEntry != null)
            {
                context.CCGroups.Remove(dbEntry);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public void SetConnectionString(string connStr)
        {
            this.context.ConnStr = connStr;
        }

        public CClayoutWithGroups GetLayoutGroupsfieldsAndValues(long lid, long cid, long grpid, string timeZone)
        {
            CClayoutWithGroups LayoutWithGroup = new CClayoutWithGroups();

            List<List<CCGroupsWithFieldsAndValues>> objLayout = new List<List<CCGroupsWithFieldsAndValues>>();

            var layout = (from lgrp in this.context.CCLayoutGroups
                          join grp in this.context.CCGroups on lgrp.GroupID equals grp.GroupID
                          join grpf in this.context.CCGroupFields on lgrp.GroupID equals grpf.GroupID
                          join ff in this.context.CCFolderFields on grpf.FieldID equals ff.FieldID
                          join fv in this.context.CCFieldValues on ff.FieldID equals fv.FieldID
                          where lgrp.LayoutID == lid && fv.ItemID == cid
                          select new { folderfield = ff.FieldName, folderfieldid = ff.FieldID, fieldvalue = fv.Value, groupname = grp.GroupName, groupid = grp.GroupID })
                     .OrderBy(gid => gid.groupid);

            var coun = layout.Count();

            if (coun > 0)
            {

                //get group id by layouts            
                var groupsid = this.context.CCLayoutGroups
                             .Where(g => g.LayoutID == lid).ToList();

                // Find all folderField
                List<CCFolderField> allFolderFields = new List<CCFolderField>();
                var fid1 = groupsid[0].FolderID;
                if (fid1 > 0) { allFolderFields = this.context.CCFolderFields.Where(id => id.FolderID == fid1).ToList(); }

                if (grpid > 0)
                {
                    var res = groupsid.Where(grp => grp.GroupID == grpid).FirstOrDefault();
                    if (res != null)
                    {
                        // remove  note group
                        groupsid.RemoveAll(exist => exist.GroupID == grpid);
                    }
                }


                List<string> grpname = new List<string>();
                foreach (var group in groupsid)  // read each group
                {

                    // var allfieldsId = CCGroupFieldRepository.CCGroupsFields;
                    var fieldsbyGroupID = this.context.CCGroupFields
                                         .Where(id => id.GroupID == group.GroupID).ToList();

                    if (fieldsbyGroupID.Count() > 0)
                    {
                        List<CCGroupsWithFieldsAndValues> fieldsAndValuesList = new List<CCGroupsWithFieldsAndValues>();

                        foreach (var groupfields in fieldsbyGroupID) // read each group field 
                        {
                            CCGroupsWithFieldsAndValues valuesAndFields = new CCGroupsWithFieldsAndValues();

                            var selectval = layout.FirstOrDefault(g => g.groupid == group.GroupID && g.folderfieldid == groupfields.FieldID);

                            if (selectval != null)
                            {

                                var fname = selectval.folderfield;
                                var fval = selectval.fieldvalue;

                                if (fname == "Start" || fname == "End") { valuesAndFields.FieldValue = GetLocalTime(timeZone, fval); }
                                else { valuesAndFields.FieldValue = fval; }
                                valuesAndFields.FieldName = fname;

                                fieldsAndValuesList.Add(valuesAndFields);
                            }
                            else
                            {
                                // Added 30Dec2014  // Display blank and new fields 
                                var fname = allFolderFields.FirstOrDefault(fid => fid.FieldID == groupfields.FieldID);
                                if (fname != null)
                                {
                                    valuesAndFields.FieldName = fname.FieldName;
                                    valuesAndFields.FieldValue = String.Empty;
                                    fieldsAndValuesList.Add(valuesAndFields);
                                }
                            }

                        }

                        objLayout.Add(fieldsAndValuesList);

                        var gname = layout.FirstOrDefault(gid => gid.groupid == group.GroupID).groupname;
                        if (gname != null) grpname.Add(gname);
                        else grpname.Add("");

                    }

                    LayoutWithGroup.LayoutWithValue = objLayout;
                    LayoutWithGroup.GroupName = grpname;

                }
            }
            return LayoutWithGroup;
        }

        private string GetLocalTime(string timeZone, string timeval)
        {
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            TimeSpan offset = tzi.GetUtcOffset(DateTime.Now);
            DateTime convertedDate = DateTime.Parse(timeval) + offset;
            string format = "yyyy-MM-dd HH:mm";
            return convertedDate.ToString(format);
        }
    }
}
