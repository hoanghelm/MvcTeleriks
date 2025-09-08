using System;
using System.Data;

using Npgsql;
using WIRS.DataAccess.Entities;
using WIRS.DataAccess.Interfaces;
using WIRS.Shared.Extensions;
using WIRS.Shared.Helpers;

namespace WIRS.DataAccess.Implementations
{
    public class IncidentDataAccess : IIncidentDataAccess
    {
        private readonly IDBHelper _dBHelper;

        public IncidentDataAccess(IDBHelper dBHelper)
        {
            _dBHelper = dBHelper;
        }

        public async Task GetRegisteredIncidents(Incident incidents)
        {
            DataSet ds = new DataSet();
            DataTable inctypedt = new DataTable();
            DataTable injurydetailsdt = new DataTable();
            DataTable medicalleavesdt = new DataTable();
            DataTable eyewitnessesdt = new DataTable();
            DataTable intervieweesdt = new DataTable();
            DataTable causeanalysisdt = new DataTable();
            DataTable ccemaildt = new DataTable();
            DataTable activityhistorydt = new DataTable();
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_RegisteredIncidents";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@incidentid", incidents.IncidentID).Direction = ParameterDirection.InputOutput;
                cmd.Parameters.Add("@incidentdate", NpgsqlTypes.NpgsqlDbType.Varchar, 22).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@incidenttime", NpgsqlTypes.NpgsqlDbType.Varchar, 4).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@sbuCode", NpgsqlTypes.NpgsqlDbType.Varchar, 4).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@division", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@department", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@location", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@incidentdesc", NpgsqlTypes.NpgsqlDbType.Varchar, 1000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@injuredname", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@injuredcontactno", NpgsqlTypes.NpgsqlDbType.Varchar, 8).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@injuredempno", NpgsqlTypes.NpgsqlDbType.Varchar, 8).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@injuredage", NpgsqlTypes.NpgsqlDbType.Varchar, 3).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@injuredrace", NpgsqlTypes.NpgsqlDbType.Char, 2).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@injuredraceoth", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@injuredsex", NpgsqlTypes.NpgsqlDbType.Char, 1).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@injurednationality", NpgsqlTypes.NpgsqlDbType.Char, 2).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@injurednricfinno", NpgsqlTypes.NpgsqlDbType.Varchar, 10).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@injuredpassport", NpgsqlTypes.NpgsqlDbType.Varchar, 10).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@injuredaddress", NpgsqlTypes.NpgsqlDbType.Varchar, 100).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@injureddesignation", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@injuredemploymenttype", NpgsqlTypes.NpgsqlDbType.Char, 2).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@injuredemploymenttypeoth", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@injuredempdate", NpgsqlTypes.NpgsqlDbType.Varchar, 22).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@superiorempname", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@superiorempno", NpgsqlTypes.NpgsqlDbType.Char, 8).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@superiorempnricno", NpgsqlTypes.NpgsqlDbType.Varchar, 10).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@superiordesignation", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@superiorempcontactno", NpgsqlTypes.NpgsqlDbType.Varchar, 8).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@injuredworknature", NpgsqlTypes.NpgsqlDbType.Varchar, 1000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@equipmentname", NpgsqlTypes.NpgsqlDbType.Varchar, 1000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@otherinfo", NpgsqlTypes.NpgsqlDbType.Varchar, 1000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@damagenearmiss", NpgsqlTypes.NpgsqlDbType.Varchar, 1000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@hodname", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@hodempno", NpgsqlTypes.NpgsqlDbType.Varchar, 8).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@hoddesignation", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@hodremarks", NpgsqlTypes.NpgsqlDbType.Varchar, 1000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@attachML", NpgsqlTypes.NpgsqlDbType.Varchar, 100).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@status", NpgsqlTypes.NpgsqlDbType.Char, 2).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@creatorname", NpgsqlTypes.NpgsqlDbType.Varchar, 50).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@creatornricno", NpgsqlTypes.NpgsqlDbType.Varchar, 10).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@creatordesignation", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@createdBy", NpgsqlTypes.NpgsqlDbType.Varchar, 8).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@interviewstatement", NpgsqlTypes.NpgsqlDbType.Varchar, 4000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@attachInterview", NpgsqlTypes.NpgsqlDbType.Varchar, 100).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@incidentimpactdesc", NpgsqlTypes.NpgsqlDbType.Varchar, 1000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@attachImpact", NpgsqlTypes.NpgsqlDbType.Varchar, 100).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@recommendactiondesc", NpgsqlTypes.NpgsqlDbType.Varchar, 4000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@attachRecommend", NpgsqlTypes.NpgsqlDbType.Varchar, 100).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@negligent", NpgsqlTypes.NpgsqlDbType.Char, 1).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@negligentcomments", NpgsqlTypes.NpgsqlDbType.Varchar, 1000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@wshoname", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@wshoempno", NpgsqlTypes.NpgsqlDbType.Varchar, 8).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@wshonric", NpgsqlTypes.NpgsqlDbType.Varchar, 10).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@wshodesignation", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@wshoremarks", NpgsqlTypes.NpgsqlDbType.Varchar, 1000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@sbuheadname", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@sbuheadempno", NpgsqlTypes.NpgsqlDbType.Varchar, 8).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@sbuheaddesignation", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@sbuheadremarks", NpgsqlTypes.NpgsqlDbType.Varchar, 1000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@submittedon", NpgsqlTypes.NpgsqlDbType.Varchar, 22).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@modifyDate", NpgsqlTypes.NpgsqlDbType.Varchar, 20).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@riskassesement", NpgsqlTypes.NpgsqlDbType.Char, 1).Direction = ParameterDirection.Output;



                NpgsqlDataAdapter da = new NpgsqlDataAdapter
                {
                    SelectCommand = cmd
                };
                da.Fill(ds);

                if (ds.Tables.Count > 0)
                {
                    inctypedt.Merge(ds.Tables[0]);
                    injurydetailsdt.Merge(ds.Tables[1]);
                    medicalleavesdt.Merge(ds.Tables[2]);
                    eyewitnessesdt.Merge(ds.Tables[3]);
                    intervieweesdt.Merge(ds.Tables[4]);
                    causeanalysisdt.Merge(ds.Tables[5]);
                    ccemaildt.Merge(ds.Tables[6]);
                    activityhistorydt.Merge(ds.Tables[7]);
                }
                incidents.IncidentTypes = inctypedt;
                incidents.InjuryDetails = injurydetailsdt;
                incidents.MedicalLeaves = medicalleavesdt;
                incidents.EyeWitnesses = eyewitnessesdt;
                incidents.Interviewees = intervieweesdt;
                incidents.Causeanalysis = causeanalysisdt;
                incidents.CCEmail = ccemaildt;
                incidents.ActivityHistory = activityhistorydt;


                incidents.IncidentID = cmd.Parameters["@incidentid"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@incidentid"].Value;
                incidents.IncidentDate = cmd.Parameters["@incidentdate"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@incidentdate"].Value;
                incidents.IncidentTime = cmd.Parameters["@incidenttime"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@incidenttime"].Value;
                incidents.SBU = cmd.Parameters["@sbuCode"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@sbuCode"].Value;
                incidents.Division = cmd.Parameters["@division"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@division"].Value;
                incidents.Department = cmd.Parameters["@department"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@department"].Value;
                incidents.Location = cmd.Parameters["@location"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@location"].Value;
                incidents.IncidentDescription = cmd.Parameters["@incidentdesc"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@incidentdesc"].Value;
                incidents.InjuredEmpName = cmd.Parameters["@injuredname"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@injuredname"].Value;
                incidents.InjuredEmpContact = cmd.Parameters["@injuredcontactno"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@injuredcontactno"].Value;
                incidents.InjuredEmpNo = cmd.Parameters["@injuredempno"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@injuredempno"].Value;
                incidents.InjuredEmpAge = cmd.Parameters["@injuredage"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@injuredage"].Value;
                incidents.InjuredEmpRace = cmd.Parameters["@injuredrace"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@injuredrace"].Value;
                incidents.InjuredEmpRaceOth = cmd.Parameters["@injuredraceoth"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@injuredraceoth"].Value;
                incidents.InjuredEmpSex = cmd.Parameters["@injuredsex"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@injuredsex"].Value;
                incidents.InjuredEmpNationality = cmd.Parameters["@injurednationality"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@injurednationality"].Value;
                incidents.InjuredEmpNRICFIN = cmd.Parameters["@injurednricfinno"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@injurednricfinno"].Value;
                incidents.InjuredEmpPassport = cmd.Parameters["@injuredpassport"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@injuredpassport"].Value;
                incidents.InjuredEmpAddress = cmd.Parameters["@injuredaddress"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@injuredaddress"].Value;
                incidents.InjuredEmpDesignation = cmd.Parameters["@injureddesignation"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@injureddesignation"].Value;
                incidents.InjuredEmpType = cmd.Parameters["@injuredemploymenttype"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@injuredemploymenttype"].Value;
                incidents.InjuredEmpTypeOth = cmd.Parameters["@injuredemploymenttypeOth"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@injuredemploymenttypeOth"].Value;
                incidents.InjuredDOE = cmd.Parameters["@injuredempdate"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@injuredempdate"].Value;
                incidents.SuperiorEmpName = cmd.Parameters["@superiorempname"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@superiorempname"].Value;
                incidents.SuperiorEmpNo = cmd.Parameters["@superiorempno"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@superiorempno"].Value;
                incidents.SuperiorEmpNRIC = cmd.Parameters["@superiorempnricno"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@superiorempnricno"].Value;
                incidents.SuperiorEmpDesignation = cmd.Parameters["@superiordesignation"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@superiordesignation"].Value;
                incidents.SuperiorEmpContactNo = cmd.Parameters["@superiorempcontactno"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@superiorempcontactno"].Value;
                incidents.InjuredAssignedWork = cmd.Parameters["@injuredworknature"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@injuredworknature"].Value;
                incidents.MachineInvolved = cmd.Parameters["@equipmentname"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@equipmentname"].Value;
                incidents.OtherRelevantInfo = cmd.Parameters["@otherinfo"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@otherinfo"].Value;
                incidents.DamageNearmissDescription = cmd.Parameters["@damagenearmiss"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@damagenearmiss"].Value;
                incidents.HODName = cmd.Parameters["@hodname"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@hodname"].Value;
                incidents.HODEmpNo = cmd.Parameters["@hodempno"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@hodempno"].Value;
                incidents.HODEmpDesignation = cmd.Parameters["@hoddesignation"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@hoddesignation"].Value;
                incidents.HODEmpRemark = cmd.Parameters["@hodremarks"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@hodremarks"].Value;
                incidents.AttachmentML = cmd.Parameters["@attachML"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@attachML"].Value;
                incidents.Status = cmd.Parameters["@status"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@status"].Value;
                incidents.CreatorEmpName = cmd.Parameters["@creatorname"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@creatorname"].Value;
                incidents.CreatorEmpNRIC = cmd.Parameters["@creatornricno"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@creatornricno"].Value;
                incidents.CreatorEmpDesignation = cmd.Parameters["@creatordesignation"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@creatordesignation"].Value;
                incidents.CreatorEmpNo = cmd.Parameters["@createdBy"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@createdBy"].Value;
                incidents.InterviewStatement = cmd.Parameters["@interviewstatement"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@interviewstatement"].Value;
                incidents.AttachmentInterview = cmd.Parameters["@attachInterview"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@attachInterview"].Value;
                incidents.IncidentImpact = cmd.Parameters["@incidentimpactdesc"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@incidentimpactdesc"].Value;
                incidents.AttachmentImpact = cmd.Parameters["@attachImpact"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@attachImpact"].Value;
                incidents.RecommendAction = cmd.Parameters["@recommendactiondesc"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@recommendactiondesc"].Value;
                incidents.AttachmentRA = cmd.Parameters["@attachRecommend"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@attachRecommend"].Value;
                incidents.Negligent = cmd.Parameters["@negligent"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@negligent"].Value;
                incidents.NegligentComment = cmd.Parameters["@negligentcomments"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@negligentcomments"].Value;
                incidents.WSHName = cmd.Parameters["@wshoname"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@wshoname"].Value;
                incidents.WSHEmpNo = cmd.Parameters["@wshoempno"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@wshoempno"].Value;
                incidents.WSHNRIC = cmd.Parameters["@wshonric"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@wshonric"].Value;
                incidents.WSHEmpDesignation = cmd.Parameters["@wshodesignation"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@wshodesignation"].Value;
                incidents.WSHEmpRemark = cmd.Parameters["@wshoremarks"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@wshoremarks"].Value;
                incidents.SBUHName = cmd.Parameters["@sbuheadname"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@sbuheadname"].Value;
                incidents.SBUHEmpNo = cmd.Parameters["@sbuheadempno"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@sbuheadempno"].Value;
                incidents.SBUHEmpDesignation = cmd.Parameters["@sbuheaddesignation"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@sbuheaddesignation"].Value;
                incidents.SBUHEmpRemark = cmd.Parameters["@sbuheadremarks"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@sbuheadremarks"].Value;
                incidents.SubmittedOn = cmd.Parameters["@submittedon"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@submittedon"].Value;
                incidents.ModifyDate = cmd.Parameters["@modifyDate"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@modifyDate"].Value;
                incidents.RiskAssessment = cmd.Parameters["@riskassesement"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@riskassesement"].Value;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();

            }
        }

        public async Task<(string incident_ID, string error_Code)> SaveIncidents(Incident incidents)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            string incident_ID = string.Empty;
            string error_Code = string.Empty;
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_insert_Incidents";
                cmd.CommandType = CommandType.StoredProcedure;

                //cmd.Parameters.AddWithValue("@incidentdate", incidents.IncidentDate == null || incidents.IncidentDate == string.Empty ? (object)DBNull.Value : (object)incidents.IncidentDate);
                //cmd.Parameters.AddWithValue("@incidenttime", incidents.IncidentTime == null || incidents.IncidentTime == string.Empty ? (object)DBNull.Value : (object)incidents.IncidentTime);
                cmd.Parameters.AddWithValue("@incidentdatetime", incidents.IncidentDateTime == null || incidents.IncidentDateTime == string.Empty ? DBNull.Value : (object)incidents.IncidentDateTime);
                cmd.Parameters.AddWithValue("@sbuCode", incidents.SBU == null || incidents.SBU == string.Empty ? DBNull.Value : (object)incidents.SBU);
                cmd.Parameters.AddWithValue("@division", incidents.Division == null || incidents.Division == string.Empty ? DBNull.Value : (object)incidents.Division);
                cmd.Parameters.AddWithValue("@department", incidents.Department == null || incidents.Department == string.Empty ? DBNull.Value : (object)incidents.Department);
                cmd.Parameters.AddWithValue("@location", incidents.Location == null || incidents.Location == string.Empty ? DBNull.Value : (object)incidents.Location);
                cmd.Parameters.AddWithValue("@incidentdesc", incidents.IncidentDescription == null || incidents.IncidentDescription == string.Empty ? DBNull.Value : (object)incidents.IncidentDescription);
                cmd.Parameters.AddWithValue("@injuredname", incidents.InjuredEmpName == null || incidents.InjuredEmpName == string.Empty ? DBNull.Value : (object)incidents.InjuredEmpName);
                cmd.Parameters.AddWithValue("@injuredcontactno", incidents.InjuredEmpContact == null || incidents.InjuredEmpContact == string.Empty ? DBNull.Value : (object)incidents.InjuredEmpContact);
                cmd.Parameters.AddWithValue("@injuredempno", incidents.InjuredEmpNo == null || incidents.InjuredEmpNo == string.Empty ? DBNull.Value : (object)incidents.InjuredEmpNo);
                cmd.Parameters.AddWithValue("@injuredage", incidents.InjuredEmpAge == null || incidents.InjuredEmpAge == string.Empty ? DBNull.Value : (object)incidents.InjuredEmpAge);
                cmd.Parameters.AddWithValue("@injuredrace", incidents.InjuredEmpRace == null || incidents.InjuredEmpRace == string.Empty ? DBNull.Value : (object)incidents.InjuredEmpRace);
                cmd.Parameters.AddWithValue("@injuredraceoth", incidents.InjuredEmpRaceOth == null || incidents.InjuredEmpRaceOth == string.Empty ? DBNull.Value : (object)incidents.InjuredEmpRaceOth);
                cmd.Parameters.AddWithValue("@injuredsex", incidents.InjuredEmpSex == null || incidents.InjuredEmpSex == string.Empty ? DBNull.Value : (object)incidents.InjuredEmpSex);
                cmd.Parameters.AddWithValue("@injurednationality", incidents.InjuredEmpNationality == null || incidents.InjuredEmpNationality == string.Empty ? DBNull.Value : (object)incidents.InjuredEmpNationality);
                cmd.Parameters.AddWithValue("@injurednricfinno", incidents.InjuredEmpNRICFIN == null || incidents.InjuredEmpNRICFIN == string.Empty ? DBNull.Value : (object)incidents.InjuredEmpNRICFIN);
                cmd.Parameters.AddWithValue("@injuredpassport", incidents.InjuredEmpPassport == null || incidents.InjuredEmpPassport == string.Empty ? DBNull.Value : (object)incidents.InjuredEmpPassport);
                cmd.Parameters.AddWithValue("@injuredaddress", incidents.InjuredEmpAddress == null || incidents.InjuredEmpAddress == string.Empty ? DBNull.Value : (object)incidents.InjuredEmpAddress);
                cmd.Parameters.AddWithValue("@injureddesignation", incidents.InjuredEmpDesignation == null || incidents.InjuredEmpDesignation == string.Empty ? DBNull.Value : (object)incidents.InjuredEmpDesignation);
                cmd.Parameters.AddWithValue("@injuredemploymenttype", incidents.InjuredEmpType == null || incidents.InjuredEmpType == string.Empty ? DBNull.Value : (object)incidents.InjuredEmpType);
                cmd.Parameters.AddWithValue("@injuredemploymenttypeOth", incidents.InjuredEmpTypeOth == null || incidents.InjuredEmpTypeOth == string.Empty ? DBNull.Value : (object)incidents.InjuredEmpTypeOth);
                cmd.Parameters.AddWithValue("@injuredempdate", incidents.InjuredDOE == null || incidents.InjuredDOE == string.Empty ? DBNull.Value : (object)incidents.InjuredDOE);

                cmd.Parameters.AddWithValue("@superiorempname", incidents.SuperiorEmpName == null || incidents.SuperiorEmpName == string.Empty ? DBNull.Value : (object)incidents.SuperiorEmpName);
                cmd.Parameters.AddWithValue("@superiorempno", incidents.SuperiorEmpNo == null || incidents.SuperiorEmpNo == string.Empty ? DBNull.Value : (object)incidents.SuperiorEmpNo);
                cmd.Parameters.AddWithValue("@superiorempnricno", incidents.SuperiorEmpNRIC == null || incidents.SuperiorEmpNRIC == string.Empty ? DBNull.Value : (object)incidents.SuperiorEmpNRIC);
                cmd.Parameters.AddWithValue("@superiordesignation", incidents.SuperiorEmpDesignation == null || incidents.SuperiorEmpDesignation == string.Empty ? DBNull.Value : (object)incidents.SuperiorEmpDesignation);
                cmd.Parameters.AddWithValue("@superiorempcontactno", incidents.SuperiorEmpContactNo == null || incidents.SuperiorEmpContactNo == string.Empty ? DBNull.Value : (object)incidents.SuperiorEmpContactNo);

                cmd.Parameters.AddWithValue("@injuredworknature", incidents.InjuredAssignedWork == null || incidents.InjuredAssignedWork == string.Empty ? DBNull.Value : (object)incidents.InjuredAssignedWork);
                cmd.Parameters.AddWithValue("@equipmentname", incidents.MachineInvolved == null || incidents.MachineInvolved == string.Empty ? DBNull.Value : (object)incidents.MachineInvolved);
                cmd.Parameters.AddWithValue("@otherinfo", incidents.OtherRelevantInfo == null || incidents.OtherRelevantInfo == string.Empty ? DBNull.Value : (object)incidents.OtherRelevantInfo);
                cmd.Parameters.AddWithValue("@damagenearmiss", incidents.DamageNearmissDescription == null || incidents.DamageNearmissDescription == string.Empty ? DBNull.Value : (object)incidents.DamageNearmissDescription);
                cmd.Parameters.AddWithValue("@hodname", incidents.HODName == null || incidents.HODName == string.Empty ? DBNull.Value : (object)incidents.HODName);
                cmd.Parameters.AddWithValue("@hodempno", incidents.HODEmpNo == null || incidents.HODEmpNo == string.Empty ? DBNull.Value : (object)incidents.HODEmpNo);
                cmd.Parameters.AddWithValue("@hoddesignation", incidents.HODEmpDesignation == null || incidents.HODEmpDesignation == string.Empty ? DBNull.Value : (object)incidents.HODEmpDesignation);
                //cmd.Parameters.AddWithValue("@hodemail", incidents.HODEmail == null || incidents.HODEmail == string.Empty ? (object)DBNull.Value : (object)incidents.HODEmail); 
                cmd.Parameters.AddWithValue("@hodremarks", incidents.HODEmpRemark == null || incidents.HODEmpRemark == string.Empty ? DBNull.Value : (object)incidents.HODEmpRemark);
                cmd.Parameters.AddWithValue("@attachML", incidents.AttachmentML == null || incidents.AttachmentML == string.Empty ? DBNull.Value : (object)incidents.AttachmentML);
                cmd.Parameters.AddWithValue("@status", incidents.Status == null || incidents.Status == string.Empty ? DBNull.Value : (object)incidents.Status);
                cmd.Parameters.AddWithValue("@activitytype", incidents.ActivityType == null || incidents.ActivityType == string.Empty ? DBNull.Value : (object)incidents.ActivityType);
                cmd.Parameters.AddWithValue("@popUpRemarks", incidents.PopUpRemarks == null || incidents.PopUpRemarks == string.Empty ? DBNull.Value : (object)incidents.PopUpRemarks);
                cmd.Parameters.AddWithValue("@creatorname", incidents.CreatorEmpName == null || incidents.CreatorEmpName == string.Empty ? DBNull.Value : (object)incidents.CreatorEmpName);
                cmd.Parameters.AddWithValue("@creatornricno", incidents.CreatorEmpNRIC == null || incidents.CreatorEmpNRIC == string.Empty ? DBNull.Value : (object)incidents.CreatorEmpNRIC);
                cmd.Parameters.AddWithValue("@creatordesignation", incidents.CreatorEmpDesignation == null || incidents.CreatorEmpDesignation == string.Empty ? DBNull.Value : (object)incidents.CreatorEmpDesignation);
                cmd.Parameters.AddWithValue("@createdBy", incidents.CreatorEmpNo == null || incidents.CreatorEmpNo == string.Empty ? DBNull.Value : (object)incidents.CreatorEmpNo);
                cmd.Parameters.AddWithValue("@incidentTypes", incidents.IncidentTypes);
                cmd.Parameters.AddWithValue("@injuryDetails", incidents.InjuryDetails);
                cmd.Parameters.AddWithValue("@medicalLeaves", incidents.MedicalLeaves);
                cmd.Parameters.AddWithValue("@eyeWitness", incidents.EyeWitnesses);
                cmd.Parameters.AddWithValue("@ccEmail", incidents.CCEmail);

                NpgsqlParameter incidentID = cmd.Parameters.Add("@incidentid", NpgsqlTypes.NpgsqlDbType.Varchar);
                incidentID.Direction = ParameterDirection.Output;
                incidentID.Size = 6;

                NpgsqlParameter modifyDate = cmd.Parameters.Add("@modifyDate", NpgsqlTypes.NpgsqlDbType.Varchar);
                modifyDate.Direction = ParameterDirection.Output;
                modifyDate.Size = 20;

                NpgsqlParameter err_Code = cmd.Parameters.Add("@errCode", NpgsqlTypes.NpgsqlDbType.Varchar);
                err_Code.Direction = ParameterDirection.Output;
                err_Code.Size = 1000;

                cmd.ExecuteNonQuery();
                incident_ID = cmd.Parameters["@incidentid"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@incidentid"].Value;
                incidents.ModifyDate = cmd.Parameters["@modifyDate"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@modifyDate"].Value;
                error_Code = cmd.Parameters["@errCode"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@errCode"].Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }

            return (incident_ID, error_Code);
        }

        public async Task<(string incident_ID, string error_Code)> UpdateIncidents(Incident incidents)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            string incident_ID = string.Empty;
            string error_Code = string.Empty;

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_update_Incidents";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@incidentdatetime", incidents.IncidentDateTime == null || incidents.IncidentDateTime == string.Empty ? DBNull.Value : (object)incidents.IncidentDateTime);
                cmd.Parameters.AddWithValue("@sbuCode", incidents.SBU == null || incidents.SBU == string.Empty ? DBNull.Value : (object)incidents.SBU);
                cmd.Parameters.AddWithValue("@division", incidents.Division == null || incidents.Division == string.Empty ? DBNull.Value : (object)incidents.Division);
                cmd.Parameters.AddWithValue("@department", incidents.Department == null || incidents.Department == string.Empty ? DBNull.Value : (object)incidents.Department);
                cmd.Parameters.AddWithValue("@location", incidents.Location == null || incidents.Location == string.Empty ? DBNull.Value : (object)incidents.Location);
                cmd.Parameters.AddWithValue("@incidentdesc", incidents.IncidentDescription == null || incidents.IncidentDescription == string.Empty ? DBNull.Value : (object)incidents.IncidentDescription);
                cmd.Parameters.AddWithValue("@injuredname", incidents.InjuredEmpName == null || incidents.InjuredEmpName == string.Empty ? DBNull.Value : (object)incidents.InjuredEmpName);
                cmd.Parameters.AddWithValue("@injuredcontactno", incidents.InjuredEmpContact == null || incidents.InjuredEmpContact == string.Empty ? DBNull.Value : (object)incidents.InjuredEmpContact);
                cmd.Parameters.AddWithValue("@injuredempno", incidents.InjuredEmpNo == null || incidents.InjuredEmpNo == string.Empty ? DBNull.Value : (object)incidents.InjuredEmpNo);
                cmd.Parameters.AddWithValue("@injuredage", incidents.InjuredEmpAge == null || incidents.InjuredEmpAge == string.Empty ? DBNull.Value : (object)incidents.InjuredEmpAge);
                cmd.Parameters.AddWithValue("@injuredrace", incidents.InjuredEmpRace == null || incidents.InjuredEmpRace == string.Empty ? DBNull.Value : (object)incidents.InjuredEmpRace);
                cmd.Parameters.AddWithValue("@injuredraceoth", incidents.InjuredEmpRaceOth == null || incidents.InjuredEmpRaceOth == string.Empty ? DBNull.Value : (object)incidents.InjuredEmpRaceOth);
                cmd.Parameters.AddWithValue("@injuredsex", incidents.InjuredEmpSex == null || incidents.InjuredEmpSex == string.Empty ? DBNull.Value : (object)incidents.InjuredEmpSex);
                cmd.Parameters.AddWithValue("@injurednationality", incidents.InjuredEmpNationality == null || incidents.InjuredEmpNationality == string.Empty ? DBNull.Value : (object)incidents.InjuredEmpNationality);
                cmd.Parameters.AddWithValue("@injurednricfinno", incidents.InjuredEmpNRICFIN == null || incidents.InjuredEmpNRICFIN == string.Empty ? DBNull.Value : (object)incidents.InjuredEmpNRICFIN);
                cmd.Parameters.AddWithValue("@injuredpassport", incidents.InjuredEmpPassport == null || incidents.InjuredEmpPassport == string.Empty ? DBNull.Value : (object)incidents.InjuredEmpPassport);
                cmd.Parameters.AddWithValue("@injuredaddress", incidents.InjuredEmpAddress == null || incidents.InjuredEmpAddress == string.Empty ? DBNull.Value : (object)incidents.InjuredEmpAddress);
                cmd.Parameters.AddWithValue("@injureddesignation", incidents.InjuredEmpDesignation == null || incidents.InjuredEmpDesignation == string.Empty ? DBNull.Value : (object)incidents.InjuredEmpDesignation);
                cmd.Parameters.AddWithValue("@injuredemploymenttype", incidents.InjuredEmpType == null || incidents.InjuredEmpType == string.Empty ? DBNull.Value : (object)incidents.InjuredEmpType);
                cmd.Parameters.AddWithValue("@injuredemploymenttypeOth", incidents.InjuredEmpTypeOth == null || incidents.InjuredEmpTypeOth == string.Empty ? DBNull.Value : (object)incidents.InjuredEmpTypeOth);
                cmd.Parameters.AddWithValue("@injuredempdate", incidents.InjuredDOE == null || incidents.InjuredDOE == string.Empty ? DBNull.Value : (object)incidents.InjuredDOE);
                cmd.Parameters.AddWithValue("@superiorempname", incidents.SuperiorEmpName == null || incidents.SuperiorEmpName == string.Empty ? DBNull.Value : (object)incidents.SuperiorEmpName);
                cmd.Parameters.AddWithValue("@superiorempno", incidents.SuperiorEmpNo == null || incidents.SuperiorEmpNo == string.Empty ? DBNull.Value : (object)incidents.SuperiorEmpNo);
                cmd.Parameters.AddWithValue("@superiorempnricno", incidents.SuperiorEmpNRIC == null || incidents.SuperiorEmpNRIC == string.Empty ? DBNull.Value : (object)incidents.SuperiorEmpNRIC);
                cmd.Parameters.AddWithValue("@superiordesignation", incidents.SuperiorEmpDesignation == null || incidents.SuperiorEmpDesignation == string.Empty ? DBNull.Value : (object)incidents.SuperiorEmpDesignation);
                cmd.Parameters.AddWithValue("@superiorempcontactno", incidents.SuperiorEmpContactNo == null || incidents.SuperiorEmpContactNo == string.Empty ? DBNull.Value : (object)incidents.SuperiorEmpContactNo);
                cmd.Parameters.AddWithValue("@injuredworknature", incidents.InjuredAssignedWork == null || incidents.InjuredAssignedWork == string.Empty ? DBNull.Value : (object)incidents.InjuredAssignedWork);
                cmd.Parameters.AddWithValue("@equipmentname", incidents.MachineInvolved == null || incidents.MachineInvolved == string.Empty ? DBNull.Value : (object)incidents.MachineInvolved);
                cmd.Parameters.AddWithValue("@otherinfo", incidents.OtherRelevantInfo == null || incidents.OtherRelevantInfo == string.Empty ? DBNull.Value : (object)incidents.OtherRelevantInfo);
                cmd.Parameters.AddWithValue("@damagenearmiss", incidents.DamageNearmissDescription == null || incidents.DamageNearmissDescription == string.Empty ? DBNull.Value : (object)incidents.DamageNearmissDescription);
                cmd.Parameters.AddWithValue("@hodname", incidents.HODName == null || incidents.HODName == string.Empty ? DBNull.Value : (object)incidents.HODName);
                cmd.Parameters.AddWithValue("@hodempno", incidents.HODEmpNo == null || incidents.HODEmpNo == string.Empty ? DBNull.Value : (object)incidents.HODEmpNo);
                cmd.Parameters.AddWithValue("@hoddesignation", incidents.HODEmpDesignation == null || incidents.HODEmpDesignation == string.Empty ? DBNull.Value : (object)incidents.HODEmpDesignation);
                cmd.Parameters.AddWithValue("@hodremarks", incidents.HODEmpRemark == null || incidents.HODEmpRemark == string.Empty ? DBNull.Value : (object)incidents.HODEmpRemark);
                cmd.Parameters.AddWithValue("@attachML", incidents.AttachmentML == null || incidents.AttachmentML == string.Empty ? DBNull.Value : (object)incidents.AttachmentML);
                cmd.Parameters.AddWithValue("@status", incidents.Status == null || incidents.Status == string.Empty ? DBNull.Value : (object)incidents.Status);
                cmd.Parameters.AddWithValue("@activitytype", incidents.ActivityType == null || incidents.ActivityType == string.Empty ? DBNull.Value : (object)incidents.ActivityType);
                cmd.Parameters.AddWithValue("@popUpRemarks", incidents.PopUpRemarks == null || incidents.PopUpRemarks == string.Empty ? DBNull.Value : (object)incidents.PopUpRemarks);
                cmd.Parameters.AddWithValue("@creatorname", incidents.CreatorEmpName == null || incidents.CreatorEmpName == string.Empty ? DBNull.Value : (object)incidents.CreatorEmpName);
                cmd.Parameters.AddWithValue("@creatornricno", incidents.CreatorEmpNRIC == null || incidents.CreatorEmpNRIC == string.Empty ? DBNull.Value : (object)incidents.CreatorEmpNRIC);
                cmd.Parameters.AddWithValue("@creatordesignation", incidents.CreatorEmpDesignation == null || incidents.CreatorEmpDesignation == string.Empty ? DBNull.Value : (object)incidents.CreatorEmpDesignation);
                cmd.Parameters.AddWithValue("@createdBy", incidents.CreatorEmpNo == null || incidents.CreatorEmpNo == string.Empty ? DBNull.Value : (object)incidents.CreatorEmpNo);
                cmd.Parameters.AddWithValue("@incidentTypes", incidents.IncidentTypes);
                cmd.Parameters.AddWithValue("@injuryDetails", incidents.InjuryDetails);
                cmd.Parameters.AddWithValue("@medicalLeaves", incidents.MedicalLeaves);
                cmd.Parameters.AddWithValue("@ccEmail", incidents.CCEmail);
                cmd.Parameters.AddWithValue("@eyeWitness", incidents.EyeWitnesses);
                cmd.Parameters.AddWithValue("@interViewees", incidents.Interviewees);
                cmd.Parameters.AddWithValue("@causeAnalysis", incidents.Causeanalysis);
                cmd.Parameters.AddWithValue("@interviewstatement", incidents.InterviewStatement == null || incidents.InterviewStatement == string.Empty ? DBNull.Value : (object)incidents.InterviewStatement);
                cmd.Parameters.AddWithValue("@attachInterview", incidents.AttachmentInterview == null || incidents.AttachmentInterview == string.Empty ? DBNull.Value : (object)incidents.AttachmentInterview);
                cmd.Parameters.AddWithValue("@incidentimpactdesc", incidents.IncidentImpact == null || incidents.IncidentImpact == string.Empty ? DBNull.Value : (object)incidents.IncidentImpact);
                cmd.Parameters.AddWithValue("@attachImpact", incidents.AttachmentImpact == null || incidents.AttachmentImpact == string.Empty ? DBNull.Value : (object)incidents.AttachmentImpact);
                cmd.Parameters.AddWithValue("@recommendactiondesc", incidents.RecommendAction == null || incidents.RecommendAction == string.Empty ? DBNull.Value : (object)incidents.RecommendAction);
                cmd.Parameters.AddWithValue("@attachRecommend", incidents.AttachmentRA == null || incidents.AttachmentRA == string.Empty ? DBNull.Value : (object)incidents.AttachmentRA);
                cmd.Parameters.AddWithValue("@negligent", incidents.Negligent == null || incidents.Negligent == string.Empty ? DBNull.Value : (object)incidents.Negligent);
                cmd.Parameters.AddWithValue("@negligentcomments", incidents.NegligentComment == null || incidents.NegligentComment == string.Empty ? DBNull.Value : (object)incidents.NegligentComment);
                cmd.Parameters.AddWithValue("@wshoempno", incidents.WSHEmpNo == null || incidents.WSHEmpNo == string.Empty ? DBNull.Value : (object)incidents.WSHEmpNo);
                cmd.Parameters.AddWithValue("@wshonric", incidents.WSHNRIC == null || incidents.WSHNRIC == string.Empty ? DBNull.Value : (object)incidents.WSHNRIC);
                cmd.Parameters.AddWithValue("@wshodesignation", incidents.WSHEmpDesignation == null || incidents.WSHEmpDesignation == string.Empty ? DBNull.Value : (object)incidents.WSHEmpDesignation);
                cmd.Parameters.AddWithValue("@wshoremarks", incidents.WSHEmpRemark == null || incidents.WSHEmpRemark == string.Empty ? DBNull.Value : (object)incidents.WSHEmpRemark);
                cmd.Parameters.AddWithValue("@sbuheadname", incidents.SBUHName == null || incidents.SBUHName == string.Empty ? DBNull.Value : (object)incidents.SBUHName);
                cmd.Parameters.AddWithValue("@sbuheadempno", incidents.SBUHEmpNo == null || incidents.SBUHEmpNo == string.Empty ? DBNull.Value : (object)incidents.SBUHEmpNo);
                cmd.Parameters.AddWithValue("@sbuheaddesignation", incidents.SBUHEmpDesignation == null || incidents.SBUHEmpDesignation == string.Empty ? DBNull.Value : (object)incidents.SBUHEmpDesignation);
                cmd.Parameters.AddWithValue("@sbuheadremarks", incidents.SBUHEmpRemark == null || incidents.SBUHEmpRemark == string.Empty ? DBNull.Value : (object)incidents.SBUHEmpRemark);
                cmd.Parameters.AddWithValue("@modifyBy", incidents.ModifyBy == null || incidents.ModifyBy == string.Empty ? DBNull.Value : (object)incidents.ModifyBy);
                cmd.Parameters.AddWithValue("@riskassessment", incidents.RiskAssessment == null || incidents.RiskAssessment == string.Empty ? DBNull.Value : (object)incidents.RiskAssessment);
                cmd.Parameters.AddWithValue("@modifyDate", incidents.ModifyDate).Direction = ParameterDirection.InputOutput;
                cmd.Parameters.AddWithValue("@incidentid", incidents.IncidentID).Direction = ParameterDirection.InputOutput;


                NpgsqlParameter err_Code = cmd.Parameters.Add("@errCode", NpgsqlTypes.NpgsqlDbType.Varchar);
                err_Code.Direction = ParameterDirection.Output;
                err_Code.Size = 1000;

                cmd.ExecuteNonQuery();
                incident_ID = cmd.Parameters["@incidentid"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@incidentid"].Value;
                incidents.ModifyDate = cmd.Parameters["@modifyDate"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@modifyDate"].Value;
                error_Code = cmd.Parameters["@errCode"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@errCode"].Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }

            return (incident_ID, error_Code);
        }

        public async Task<DataSet> SearchIncidents(User user, Incident incidents)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            DataSet ds = new DataSet();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_search_Incidents";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@useid", user.UserId);
                cmd.Parameters.AddWithValue("@userrolecode", user.UserRole);
                cmd.Parameters.AddWithValue("@incidentid", incidents.IncidentID);
                cmd.Parameters.AddWithValue("@sbu", incidents.SBU);
                cmd.Parameters.AddWithValue("@division", incidents.Division);
                cmd.Parameters.AddWithValue("@incdatefrom", incidents.IncidentFromDate);
                cmd.Parameters.AddWithValue("@incdateto", incidents.IncidentToDate);
                cmd.Parameters.AddWithValue("@incidenttype", incidents.IncidentTypes);
                NpgsqlDataAdapter da = new NpgsqlDataAdapter
                {
                    SelectCommand = cmd
                };
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }

            return ds;
        }

        public async Task<DataSet> SearchVerfiedIncidents(User user, Incident incidents)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            DataSet ds = new DataSet();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_search_VerifiedIncidents";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@useid", user.UserId);
                cmd.Parameters.AddWithValue("@userrolecode", user.UserRole);
                cmd.Parameters.AddWithValue("@incidentid", incidents.IncidentID);
                cmd.Parameters.AddWithValue("@sbu", incidents.SBU);
                cmd.Parameters.AddWithValue("@division", incidents.Division);
                cmd.Parameters.AddWithValue("@incdatefrom", incidents.IncidentFromDate);
                cmd.Parameters.AddWithValue("@incdateto", incidents.IncidentToDate);
                cmd.Parameters.AddWithValue("@incidenttype", incidents.IncidentTypes);
                NpgsqlDataAdapter da = new NpgsqlDataAdapter
                {
                    SelectCommand = cmd
                };
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }

            return ds;
        }

        public async Task<string> GetRegisteredIncidentsByLink(Incident incidents)
        {
            DataSet ds = new DataSet();
            string error_Code = string.Empty;
            DataTable inctypedt = new DataTable();
            DataTable injurydetailsdt = new DataTable();
            DataTable medicalleavesdt = new DataTable();
            DataTable eyewitnessesdt = new DataTable();
            DataTable intervieweesdt = new DataTable();
            DataTable causeanalysisdt = new DataTable();
            DataTable ccemaildt = new DataTable();
            DataTable activityhistorydt = new DataTable();
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_RegisteredIncidentsByLink";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@incidentid", incidents.IncidentID).Direction = ParameterDirection.InputOutput;
                cmd.Parameters.Add("@incidentdate", NpgsqlTypes.NpgsqlDbType.Varchar, 22).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@incidenttime", NpgsqlTypes.NpgsqlDbType.Varchar, 4).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@sbuCode", NpgsqlTypes.NpgsqlDbType.Varchar, 4).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@division", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@department", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@location", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@incidentdesc", NpgsqlTypes.NpgsqlDbType.Varchar, 1000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@injuredname", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@injuredcontactno", NpgsqlTypes.NpgsqlDbType.Varchar, 8).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@injuredempno", NpgsqlTypes.NpgsqlDbType.Varchar, 8).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@injuredage", NpgsqlTypes.NpgsqlDbType.Varchar, 3).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@injuredrace", NpgsqlTypes.NpgsqlDbType.Char, 2).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@injuredraceoth", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@injuredsex", NpgsqlTypes.NpgsqlDbType.Char, 1).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@injurednationality", NpgsqlTypes.NpgsqlDbType.Char, 2).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@injurednricfinno", NpgsqlTypes.NpgsqlDbType.Varchar, 10).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@injuredpassport", NpgsqlTypes.NpgsqlDbType.Varchar, 10).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@injuredaddress", NpgsqlTypes.NpgsqlDbType.Varchar, 100).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@injureddesignation", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@injuredemploymenttype", NpgsqlTypes.NpgsqlDbType.Char, 2).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@injuredemploymenttypeoth", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@injuredempdate", NpgsqlTypes.NpgsqlDbType.Varchar, 22).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@superiorempname", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@superiorempno", NpgsqlTypes.NpgsqlDbType.Varchar, 8).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@superiorempnricno", NpgsqlTypes.NpgsqlDbType.Varchar, 10).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@superiordesignation", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@superiorempcontactno", NpgsqlTypes.NpgsqlDbType.Varchar, 8).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@injuredworknature", NpgsqlTypes.NpgsqlDbType.Varchar, 1000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@equipmentname", NpgsqlTypes.NpgsqlDbType.Varchar, 1000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@otherinfo", NpgsqlTypes.NpgsqlDbType.Varchar, 1000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@damagenearmiss", NpgsqlTypes.NpgsqlDbType.Varchar, 1000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@hodname", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@hodempno", NpgsqlTypes.NpgsqlDbType.Varchar, 8).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@hoddesignation", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@hodremarks", NpgsqlTypes.NpgsqlDbType.Varchar, 1000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@attachML", NpgsqlTypes.NpgsqlDbType.Varchar, 100).Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@status", incidents.Status).Direction = ParameterDirection.InputOutput;
                // cmd.Parameters.Add("@status", SqlDbType.NVarChar, 2).Direction = ParameterDirection.InputOutput;
                cmd.Parameters.Add("@creatorname", NpgsqlTypes.NpgsqlDbType.Varchar, 50).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@creatornricno", NpgsqlTypes.NpgsqlDbType.Varchar, 10).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@creatordesignation", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@createdBy", NpgsqlTypes.NpgsqlDbType.Varchar, 8).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@interviewstatement", NpgsqlTypes.NpgsqlDbType.Varchar, 4000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@attachInterview", NpgsqlTypes.NpgsqlDbType.Varchar, 100).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@incidentimpactdesc", NpgsqlTypes.NpgsqlDbType.Varchar, 1000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@attachImpact", NpgsqlTypes.NpgsqlDbType.Varchar, 100).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@recommendactiondesc", NpgsqlTypes.NpgsqlDbType.Varchar, 4000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@attachRecommend", NpgsqlTypes.NpgsqlDbType.Varchar, 100).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@negligent", NpgsqlTypes.NpgsqlDbType.Char, 1).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@negligentcomments", NpgsqlTypes.NpgsqlDbType.Varchar, 1000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@wshoname", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@wshoempno", NpgsqlTypes.NpgsqlDbType.Varchar, 8).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@wshonric", NpgsqlTypes.NpgsqlDbType.Varchar, 10).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@wshodesignation", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@wshoremarks", NpgsqlTypes.NpgsqlDbType.Varchar, 1000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@sbuheadname", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@sbuheadempno", NpgsqlTypes.NpgsqlDbType.Varchar, 8).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@sbuheaddesignation", NpgsqlTypes.NpgsqlDbType.Varchar, 40).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@sbuheadremarks", NpgsqlTypes.NpgsqlDbType.Varchar, 1000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@submittedon", NpgsqlTypes.NpgsqlDbType.Varchar, 22).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@riskassessment", NpgsqlTypes.NpgsqlDbType.Char, 1).Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@modifyDate", incidents.ModifyDate).Direction = ParameterDirection.InputOutput;



                NpgsqlParameter err_Code = cmd.Parameters.Add("@errCode", NpgsqlTypes.NpgsqlDbType.Varchar);
                err_Code.Direction = ParameterDirection.Output;
                err_Code.Size = 15;

                NpgsqlDataAdapter da = new NpgsqlDataAdapter
                {
                    SelectCommand = cmd
                };
                da.Fill(ds);

                if (ds.Tables.Count > 0)
                {
                    inctypedt.Merge(ds.Tables[0]);
                    injurydetailsdt.Merge(ds.Tables[1]);
                    medicalleavesdt.Merge(ds.Tables[2]);
                    eyewitnessesdt.Merge(ds.Tables[3]);
                    intervieweesdt.Merge(ds.Tables[4]);
                    causeanalysisdt.Merge(ds.Tables[5]);
                    ccemaildt.Merge(ds.Tables[6]);
                    activityhistorydt.Merge(ds.Tables[7]);
                }

                incidents.IncidentTypes = inctypedt;
                incidents.InjuryDetails = injurydetailsdt;
                incidents.MedicalLeaves = medicalleavesdt;
                incidents.EyeWitnesses = eyewitnessesdt;
                incidents.Interviewees = intervieweesdt;
                incidents.Causeanalysis = causeanalysisdt;
                incidents.CCEmail = ccemaildt;
                incidents.ActivityHistory = activityhistorydt;


                incidents.IncidentID = cmd.Parameters["@incidentid"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@incidentid"].Value;
                incidents.IncidentDate = cmd.Parameters["@incidentdate"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@incidentdate"].Value;
                incidents.IncidentTime = cmd.Parameters["@incidenttime"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@incidenttime"].Value;
                incidents.SBU = cmd.Parameters["@sbuCode"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@sbuCode"].Value;
                incidents.Division = cmd.Parameters["@division"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@division"].Value;
                incidents.Department = cmd.Parameters["@department"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@department"].Value;
                incidents.Location = cmd.Parameters["@location"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@location"].Value;
                incidents.IncidentDescription = cmd.Parameters["@incidentdesc"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@incidentdesc"].Value;
                incidents.InjuredEmpName = cmd.Parameters["@injuredname"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@injuredname"].Value;
                incidents.InjuredEmpContact = cmd.Parameters["@injuredcontactno"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@injuredcontactno"].Value;
                incidents.InjuredEmpNo = cmd.Parameters["@injuredempno"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@injuredempno"].Value;
                incidents.InjuredEmpAge = cmd.Parameters["@injuredage"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@injuredage"].Value;
                incidents.InjuredEmpRace = cmd.Parameters["@injuredrace"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@injuredrace"].Value;
                incidents.InjuredEmpRaceOth = cmd.Parameters["@injuredraceoth"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@injuredraceoth"].Value;
                incidents.InjuredEmpSex = cmd.Parameters["@injuredsex"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@injuredsex"].Value;
                incidents.InjuredEmpNationality = cmd.Parameters["@injurednationality"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@injurednationality"].Value;
                incidents.InjuredEmpNRICFIN = cmd.Parameters["@injurednricfinno"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@injurednricfinno"].Value;
                incidents.InjuredEmpPassport = cmd.Parameters["@injuredpassport"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@injuredpassport"].Value;
                incidents.InjuredEmpAddress = cmd.Parameters["@injuredaddress"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@injuredaddress"].Value;
                incidents.InjuredEmpDesignation = cmd.Parameters["@injureddesignation"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@injureddesignation"].Value;
                incidents.InjuredEmpType = cmd.Parameters["@injuredemploymenttype"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@injuredemploymenttype"].Value;
                incidents.InjuredEmpTypeOth = cmd.Parameters["@injuredemploymenttypeOth"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@injuredemploymenttypeOth"].Value;
                incidents.InjuredDOE = cmd.Parameters["@injuredempdate"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@injuredempdate"].Value;
                incidents.SuperiorEmpName = cmd.Parameters["@superiorempname"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@superiorempname"].Value;
                incidents.SuperiorEmpNo = cmd.Parameters["@superiorempno"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@superiorempno"].Value;
                incidents.SuperiorEmpNRIC = cmd.Parameters["@superiorempnricno"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@superiorempnricno"].Value;
                incidents.SuperiorEmpDesignation = cmd.Parameters["@superiordesignation"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@superiordesignation"].Value;
                incidents.SuperiorEmpContactNo = cmd.Parameters["@superiorempcontactno"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@superiorempcontactno"].Value;
                incidents.InjuredAssignedWork = cmd.Parameters["@injuredworknature"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@injuredworknature"].Value;
                incidents.MachineInvolved = cmd.Parameters["@equipmentname"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@equipmentname"].Value;
                incidents.OtherRelevantInfo = cmd.Parameters["@otherinfo"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@otherinfo"].Value;
                incidents.DamageNearmissDescription = cmd.Parameters["@damagenearmiss"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@damagenearmiss"].Value;
                incidents.HODName = cmd.Parameters["@hodname"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@hodname"].Value;
                incidents.HODEmpNo = cmd.Parameters["@hodempno"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@hodempno"].Value;
                incidents.HODEmpDesignation = cmd.Parameters["@hoddesignation"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@hoddesignation"].Value;
                incidents.HODEmpRemark = cmd.Parameters["@hodremarks"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@hodremarks"].Value;
                incidents.AttachmentML = cmd.Parameters["@attachML"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@attachML"].Value;
                incidents.Status = cmd.Parameters["@status"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@status"].Value;
                incidents.CreatorEmpName = cmd.Parameters["@creatorname"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@creatorname"].Value;
                incidents.CreatorEmpNRIC = cmd.Parameters["@creatornricno"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@creatornricno"].Value;
                incidents.CreatorEmpDesignation = cmd.Parameters["@creatordesignation"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@creatordesignation"].Value;
                incidents.CreatorEmpNo = cmd.Parameters["@createdBy"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@createdBy"].Value;
                incidents.InterviewStatement = cmd.Parameters["@interviewstatement"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@interviewstatement"].Value;
                incidents.AttachmentInterview = cmd.Parameters["@attachInterview"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@attachInterview"].Value;
                incidents.IncidentImpact = cmd.Parameters["@incidentimpactdesc"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@incidentimpactdesc"].Value;
                incidents.AttachmentImpact = cmd.Parameters["@attachImpact"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@attachImpact"].Value;
                incidents.RecommendAction = cmd.Parameters["@recommendactiondesc"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@recommendactiondesc"].Value;
                incidents.AttachmentRA = cmd.Parameters["@attachRecommend"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@attachRecommend"].Value;
                incidents.Negligent = cmd.Parameters["@negligent"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@negligent"].Value;
                incidents.NegligentComment = cmd.Parameters["@negligentcomments"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@negligentcomments"].Value;
                incidents.WSHName = cmd.Parameters["@wshoname"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@wshoname"].Value;
                incidents.WSHEmpNo = cmd.Parameters["@wshoempno"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@wshoempno"].Value;
                incidents.WSHNRIC = cmd.Parameters["@wshonric"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@wshonric"].Value;
                incidents.WSHEmpDesignation = cmd.Parameters["@wshodesignation"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@wshodesignation"].Value;
                incidents.WSHEmpRemark = cmd.Parameters["@wshoremarks"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@wshoremarks"].Value;
                incidents.SBUHName = cmd.Parameters["@sbuheadname"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@sbuheadname"].Value;
                incidents.SBUHEmpNo = cmd.Parameters["@sbuheadempno"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@sbuheadempno"].Value;
                incidents.SBUHEmpDesignation = cmd.Parameters["@sbuheaddesignation"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@sbuheaddesignation"].Value;
                incidents.SBUHEmpRemark = cmd.Parameters["@sbuheadremarks"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@sbuheadremarks"].Value;
                incidents.SubmittedOn = cmd.Parameters["@submittedon"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@submittedon"].Value;
                incidents.ModifyDate = cmd.Parameters["@modifyDate"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@modifyDate"].Value;
                incidents.RiskAssessment = cmd.Parameters["@riskassessment"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@riskassessment"].Value;
                error_Code = cmd.Parameters["@errCode"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@errCode"].Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }

            return error_Code;
        }

        public async Task<DataSet> GetToEmailDistributionBySBU(Incident incidents)
        {
            DataSet ds = new DataSet();
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_ToEmailBySBU";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@sbuCode", incidents.SBU);
                cmd.Parameters.AddWithValue("@incidentTypes", incidents.IncidentTypes);
                NpgsqlDataAdapter da = new NpgsqlDataAdapter
                {
                    SelectCommand = cmd
                };
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }

            return ds;
        }


        public async Task<DataSet> GetSelCCEmailListByIncidents(string incidentId, string recipienttype)
        {
            DataSet ds = new DataSet();
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_CCEmailListByIncidents";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@incidentid", incidentId);
                cmd.Parameters.AddWithValue("@recipienttype", recipienttype);
                NpgsqlDataAdapter da = new NpgsqlDataAdapter
                {
                    SelectCommand = cmd
                };
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }

            return ds;
        }

        public async Task<DataSet> GetWSHOEmailListBySBU(Incident incident)
        {
            DataSet ds = new DataSet();
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_WSHOEmailListBySBU";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@sbuCode", incident.SBU);
                cmd.Parameters.AddWithValue("@incidentTypes", incident.IncidentTypes);
                NpgsqlDataAdapter da = new NpgsqlDataAdapter
                {
                    SelectCommand = cmd
                };
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }

            return ds;
        }
        public async Task<(string IncidentDateTime, string SubmittedDateTime)> GetSubmittedDate(string incident_ID)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            string IncidentDateTime = string.Empty;
            string SubmittedDateTime = string.Empty;

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "spc_get_SubmittedDate";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@incidentid", incident_ID);

                NpgsqlParameter incDatetime = cmd.Parameters.Add("@incident_datetime", NpgsqlTypes.NpgsqlDbType.Varchar);
                incDatetime.Direction = ParameterDirection.Output;
                incDatetime.Size = 25;

                NpgsqlParameter subDatetime = cmd.Parameters.Add("@submitteddate", NpgsqlTypes.NpgsqlDbType.Varchar);
                subDatetime.Direction = ParameterDirection.Output;
                subDatetime.Size = 25;

                cmd.ExecuteNonQuery();
                IncidentDateTime = cmd.Parameters["@incident_datetime"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@incident_datetime"].Value;
                SubmittedDateTime = cmd.Parameters["@submitteddate"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@submitteddate"].Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }

            return (IncidentDateTime, SubmittedDateTime);
        }
    }
}