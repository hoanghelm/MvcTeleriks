using System.Data;


namespace WIRS.DataAccess.Entities
{
    public class WorkflowIncident
    {
        public string incident_id { get; set; }
        public DataTable incident_type { get; set; }
        public string incident_datetime { get; set; }
        public string incident_time { get; set; }
        public string incident_date { get; set; }
        public string sba_code { get; set; }
        public string sbu_code { get; set; }
        public string sbu_name { get; set; }
        public string division { get; set; }
        public string department { get; set; }
        public string department_name { get; set; }
        public string location { get; set; }
        public string location_name { get; set; }
        public string exact_location { get; set; }
        public string incident_desc { get; set; }

        public string superior_name { get; set; }
        public string superior_emp_no { get; set; }
        public string superior_designation { get; set; }


        public string negligent { get; set; }
        public string negligent_comments { get; set; }
        public string recommend_action_desc { get; set; }
        public string risk_assessment_review { get; set; }
        public string risk_assessment_review_desc { get; set; }
        public string risk_assessment_review_comments { get; set; }
        public string what_happened_and_why_comments { get; set; }

        public string status { get; set; }
        public string hod_id { get; set; }
        public string ahod_id { get; set; }
        public string wsho_id { get; set; }
        public string awsho_id { get; set; }
        public string hhod_id { get; set; }
        public string cwsho_id { get; set; }
        public string workflow_id { get; set; }
        public string created_by { get; set; }
        public string creation_date { get; set; }
        public string modified_by { get; set; }
        public string modify_date { get; set; }



        /*Added on Phase II on 5 Apr 2019*/
        public int? any_eyewitness { get; set; }
        public string damage_description { get; set; }
        public string is_working_overtime { get; set; }
        public string is_jobrelated { get; set; }
        public string examined_hospital_clinic_name { get; set; }
        public string official_working_hrs { get; set; }
        public string injured_case_type { get; set; }
    }


    public class Incident
    {

        public string IncidentID { get; set; }
        public DataTable IncidentTypes { get; set; }
        public string IncidentDate { get; set; }
        public string IncidentTime { get; set; }
        public string IncidentDateTime { get; set; }
        public string SBU { get; set; }
        public string Division { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
        public string IncidentDescription { get; set; }
        public string SuperiorEmpName { get; set; }
        public string SuperiorEmpNo { get; set; }

        public string Negligent { get; set; }
        public string NegligentComment { get; set; }

        public string RiskAssessment { get; set; }
        public string RiskAssessmentRemarks { get; set; }

        public string InjuredEmpName { get; set; }
        public string InjuredEmpContact { get; set; }
        public string InjuredEmpNo { get; set; }
        public string InjuredEmpAge { get; set; }
        public string InjuredEmpRace { get; set; }
        public string InjuredEmpRaceOth { get; set; }
        public string InjuredEmpDesignation { get; set; }
        public string InjuredEmpSex { get; set; }
        public string InjuredEmpType { get; set; }
        public string InjuredEmpTypeOth { get; set; }
        public string InjuredEmpPassport { get; set; }
        public string InjuredDOE { get; set; }
        public string InjuredEmpNationality { get; set; }
        public string InjuredEmpNRICFIN { get; set; }
        //public String InjuredEmpFIN { get; set; }
        public string InjuredEmpAddress { get; set; }
        public DataTable InjuryDetails { get; set; }
        public DataTable MedicalLeaves { get; set; }

        public string SuperiorEmpNRIC { get; set; }
        public string SuperiorEmpDesignation { get; set; }
        public string SuperiorEmpContactNo { get; set; }
        public DataTable EyeWitnesses { get; set; }
        public string InjuredAssignedWork { get; set; }
        public string MachineInvolved { get; set; }
        public string OtherRelevantInfo { get; set; }
        public string DamageNearmissDescription { get; set; }

        public string CreatorEmpName { get; set; }
        public string CreatorEmpNo { get; set; }
        public string CreatorEmpDesignation { get; set; }
        public string CreatorEmpNRIC { get; set; }

        public DataTable CCEmail { get; set; }

        public string HODName { get; set; }
        public string HODEmpNo { get; set; }
        public string HODEmpDesignation { get; set; }
        public string HODEmpRemark { get; set; }

        public string CreationDate { get; set; }
        public string ModifyBy { get; set; }
        public string ModifyDate { get; set; }
        public string Status { get; set; }

        public string PopUpRemarks { get; set; }
        public string ActivityType { get; set; }
        public DataTable ActivityHistory { get; set; }

        public string AttachmentML { get; set; }
        public DataTable Interviewees { get; set; }
        public DataTable Causeanalysis { get; set; }
        public string InterviewStatement { get; set; }
        public string AttachmentInterview { get; set; }
        public string IncidentImpact { get; set; }
        public string AttachmentImpact { get; set; }
        public string RecommendAction { get; set; }
        public string AttachmentRA { get; set; }


        public string WSHName { get; set; }
        public string WSHEmpNo { get; set; }
        public string WSHNRIC { get; set; }
        public string WSHEmpDesignation { get; set; }
        public string WSHEmpRemark { get; set; }


        public string SBUHName { get; set; }
        public string SBUHEmpNo { get; set; }
        public string SBUHEmpDesignation { get; set; }
        public string SBUHEmpRemark { get; set; }

        public string SubmittedOn { get; set; }
        public string SubmittedOnDate { get; set; }

        public string IncidentFromDate { get; set; }
        public string IncidentToDate { get; set; }

        public DataTable InvCCEmail { get; set; }
    }
}