//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GestionCaisse_MVVM
{
    using System;
    using System.Collections.Generic;
    
    public partial class historique
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public historique()
        {
            this.client1 = new HashSet<client>();
        }
    
        public int idVente { get; set; }
        public Nullable<int> idUtilisateur { get; set; }
        public int idProduit { get; set; }
        public long quantite { get; set; }
        public int idBDEAcheteur { get; set; }
        public Nullable<int> idClient { get; set; }
        public System.DateTime dateVente { get; set; }
    
        public virtual BDE bde { get; set; }
        public virtual client client { get; set; }
        public virtual produit produit { get; set; }
        public virtual utilisateur utilisateur { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<client> client1 { get; set; }
    }
}
