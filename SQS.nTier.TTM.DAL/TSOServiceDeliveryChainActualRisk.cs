using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQS.nTier.TTM.DAL
{
    using GenericFramework;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("TSOServiceDeliveryChainActualRisk")]
    public  class TSOServiceDeliveryChainActualRisk:IBaseEntity
    {
        [Key, Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public int WeekNumber { get; set; }

        [Required]
        public int Year { get; set; }

        public int ActualOperationalRiskId { get; set; }

        public virtual OperationalRisk ActualOperationalRisk { get; set; }

        public int ActualOperationalRiskIndicatorId { get; set; }

        public virtual OperationalRiskIndicator ActualOperationalRiskIndicator { get; set; }

        [StringLength(500)]
        public string ActualOperationalRiskDescription { get; set; }

        [StringLength(500)]
        public string ActualOperationalRiskMitigation { get; set; }

        [StringLength(500)]
        public string Impact { get; set; }

        [StringLength(500)]
        public string Resolution { get; set; }

        public int ResponsiblePersonId { get; set; }

        public int RaisedById { get; set; }

        public int StatusId { get; set; }

        public DateTime DueDate { get; set; }

        public virtual User ResponsiblePerson { get; set; }

        public virtual User RaisedBy { get; set; }

        public virtual RiskStatus Status { get; set; }

        public int TSOServiceDeliveryChainId { get; set; }

        public virtual TSOServiceDeliveryChain TSOServiceDeliveryChain { get; set; }

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        [Required]
        [StringLength(50)]
        public string UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        public int Version { get; set; }      

        [NotMapped]
        public ObjectSate ObjectSate
        {
            get; set;
        }        
    }
}
