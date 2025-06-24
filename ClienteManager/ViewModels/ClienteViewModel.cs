using ClienteManager.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClienteManager.ViewModels
{
    public class ClienteViewModel
    {
        public Cliente Cliente { get; set; } = new Cliente();
        public SelectList? Cidades { get; set; }
        public IFormFile? Imagem { get; set; }
    }
}