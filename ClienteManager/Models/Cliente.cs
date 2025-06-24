using System.ComponentModel.DataAnnotations;

namespace ClienteManager.Models
{
    public class Cliente
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "O CPF/CNPJ é obrigatório.")]
        [Display(Name = "CPF/CNPJ")]
        public string CodigoFiscal { get; set; } = string.Empty;

        [StringLength(15)]
        [Display(Name = "Inscrição Estadual")]
        public string? InscricaoEstatudal { get; set; }

        [Required(ErrorMessage = "O Nome é obrigatório.")]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Display(Name = "Nome Fantasia")]
        [MaxLength(100)]
        public string? NomeFantasia { get; set; }

        [Required(ErrorMessage = "O Endereço é obrigatório.")]
        [MaxLength(150)]
        public string Endereco { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Número é obrigatório.")]
        [MaxLength(10)]
        public string Numero { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Bairro é obrigatório.")]
        [MaxLength(50)]
        public string Bairro { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Cidade é obrigatória.")]
        public string Cidade { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "O Estado é obrigatório.")]
        [MaxLength(2)]
        public string Estado { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Data é obrigatória.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento / Abertura")]
        public DateTime DataNascimento { get; set; }

        [Display(Name = "Imagem (Foto/Logo)")]
        public string? ImagemUrl { get; set; }
    }
}