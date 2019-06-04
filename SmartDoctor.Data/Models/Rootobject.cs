using System;
using System.Collections.Generic;

namespace SmartDoctor.Data.Models
{

    public class Rootobject
    {
        public List<Drug> Property1 { get; set; }
    }

    public class Drug
    {
        public int Id { get; set; }
        public Approvalstatus ApprovalStatus { get; set; }
        public Class2 Class { get; set; }
        public List<Name> Names { get; set; }
        public List<Company> Companies { get; set; }
        public List<Disease> Diseases { get; set; }
        public string Formula { get; set; }
        public string PhaseOfDevelopment { get; set; }
        public List<Image> Images { get; set; }
        public Registrynumbers RegistryNumbers { get; set; }
        public Approveduse ApprovedUse { get; set; }
        public string ChemicalName { get; set; }
        public string ChemicalClass { get; set; }
        public List<Patientversion> PatientVersions { get; set; }
        public Healthprofessionalversions HealthProfessionalVersions { get; set; }
        public string CompoundDetailsSource { get; set; }
    }

    public class Approvalstatus
    {
        public string English { get; set; }
        public string Spanish { get; set; }
    }

    public class Class2
    {
        public string English { get; set; }
        public string Spanish { get; set; }
    }

    public class Registrynumbers
    {
        public List<Registry> Registries { get; set; }
    }

    public class Registry
    {
        public string ID { get; set; }
        public string Type { get; set; }
        public string MolecularWeight { get; set; }
        public string ChemicalStructureImageURL { get; set; }
    }

    public class Approveduse
    {
        public string English { get; set; }
        public string Spanish { get; set; }
    }

    public class Healthprofessionalversions
    {
        public List<Fdalabel> FDALabels { get; set; }
        public List<Healthprofessionalversion> HealthProfessionalVersion { get; set; }
    }

    public class Fdalabel
    {
        public string Title { get; set; }
        public string Code { get; set; }
        public string URL { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string SPLVersion { get; set; }
        public string SetID { get; set; }
    }

    public class Healthprofessionalversion
    {
        public string Language { get; set; }
        public List<Template> Template { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime ReviewedDate { get; set; }
    }

    public class Template
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }

    public class Name
    {
        public string Title { get; set; }
        public string Type { get; set; }
        public string Audio { get; set; }
    }

    public class Company
    {
        public string Name { get; set; }
        public string Phone { get; set; }
    }

    public class Disease
    {
        public string English { get; set; }
        public string Spanish { get; set; }
    }

    public class Image
    {
        public string Name { get; set; }
        public string URL { get; set; }
    }

    public class Patientversion
    {
        public string Language { get; set; }
        public List<Template1> Template { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime ReviewedDate { get; set; }
    }

    public class Template1
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }

}
