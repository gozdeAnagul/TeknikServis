﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TeknikServis.Models.Abstracts;
using TeknikServis.Models.IdentityModels;

namespace TeknikServis.Models.Entities
{
    [Table("Arizalar")]
    public class Ariza : RepositoryBase<int>
    {
        [StringLength(200)]
        public string Aciklama { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? ArizaBaslangicTarihi { get; set; }
        public DateTime? ArizaBitisTarihi { get; set; }
        public bool ArizaOnaylandiMi { get; set; }
        [StringLength(200)]
        public string Adres { get; set; }
        public string MusteriId { get; set; }
        public int MarkaId { get; set; }
        public int ModelId { get; set; }
        public string TeknisyenId { get; set; }
        public bool ArizaYapildiMi { get; set; }
        public List<string> ArizaFoto { get; set; }
        public bool GarantiliVarMi { get; set; }
        public int Ucret { get; set; }

        [ForeignKey("MusteriId")]
        public virtual User Musteri { get; set; }
        [ForeignKey("TeknisyenId")]
        public virtual User Teknisyen { get; set; }
        [ForeignKey("ModelId")]
        public virtual Model Model { get; set; }
        public virtual List<Fotograf> Fotograflar { get; set; } = new List<Fotograf>();
    }
}
