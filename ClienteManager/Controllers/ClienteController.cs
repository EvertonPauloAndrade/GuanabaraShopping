using Microsoft.AspNetCore.Mvc;
using ClienteManager.Data;
using ClienteManager.Models;
using ClienteManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace ClienteManager.Controllers
{
    [Authorize]
    public class ClienteController : Controller
    {
        private readonly JsonDataService _dataService;
        private readonly IWebHostEnvironment _env;

        public ClienteController(JsonDataService dataService, IWebHostEnvironment env)
        {
            _dataService = dataService;
            _env = env;
        }

        public IActionResult Index()
        {
            var clientes = _dataService.GetClients();
            return View(clientes);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new ClienteViewModel
            {
                Cidades = new SelectList(_dataService.GetCities())
            };
            return View("CreateEdit", viewModel);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var cliente = _dataService.GetClients().FirstOrDefault(c => c.ID == id);
            if (cliente == null) return NotFound();

            var viewModel = new ClienteViewModel
            {
                Cliente = cliente,
                Cidades = new SelectList(_dataService.GetCities(), cliente.Cidade)
            };
            return View("CreateEdit", viewModel);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(ClienteViewModel viewModel)
        {
            try
            {
                var allClients = _dataService.GetClients();

                // Validação única do CodigoFiscal
                if (allClients.Any(c => c.CodigoFiscal == viewModel.Cliente.CodigoFiscal && c.ID != viewModel.Cliente.ID))
                    ModelState.AddModelError("Cliente.CodigoFiscal", "Este CPF/CNPJ já está cadastrado.");

                // Validação única da Inscrição Estadual (se preenchida)
                if (!string.IsNullOrEmpty(viewModel.Cliente.InscricaoEstatudal) &&
                    allClients.Any(c => c.InscricaoEstatudal == viewModel.Cliente.InscricaoEstatudal && c.ID != viewModel.Cliente.ID))
                    ModelState.AddModelError("Cliente.InscricaoEstatudal", "Esta Inscrição Estadual já está cadastrada.");

                // Validação da imagem (se foi enviada)
                if (viewModel.Imagem != null)
                {
                    var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                    if (!allowedTypes.Contains(viewModel.Imagem.ContentType))
                    {
                        ModelState.AddModelError("Imagem", "Tipo de imagem não permitido. Apenas JPG, PNG e GIF são aceitos.");
                    }
                }

                if (!ModelState.IsValid)
                {
                    viewModel.Cidades = new SelectList(_dataService.GetCities(), viewModel.Cliente.Cidade);
                    return View("CreateEdit", viewModel);
                }

                // Upload da imagem
                if (viewModel.Imagem != null)
                {
                    if (viewModel.Cliente.ID != 0 && !string.IsNullOrEmpty(viewModel.Cliente.ImagemUrl))
                    {
                        var oldImagePath = Path.Combine(_env.WebRootPath ?? throw new Exception("WebRootPath não configurado"), viewModel.Cliente.ImagemUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                            System.IO.File.Delete(oldImagePath);
                    }

                    string uploadsDir = Path.Combine(_env.WebRootPath ?? throw new Exception("WebRootPath não configurado"), "uploads");
                    if (!Directory.Exists(uploadsDir))
                        Directory.CreateDirectory(uploadsDir);

                    var safeFileName = Path.GetFileName(viewModel.Imagem.FileName);
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + safeFileName;
                    string filePath = Path.Combine(uploadsDir, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.Imagem.CopyToAsync(fileStream);
                    }
                    viewModel.Cliente.ImagemUrl = "/uploads/" + uniqueFileName;
                }

                if (viewModel.Cliente.ID == 0)
                {
                    viewModel.Cliente.ID = allClients.Any() ? allClients.Max(c => c.ID) + 1 : 1;
                    allClients.Add(viewModel.Cliente);
                }
                else
                {
                    var clienteToUpdate = allClients.FirstOrDefault(c => c.ID == viewModel.Cliente.ID);
                    if (clienteToUpdate != null)
                    {
                        clienteToUpdate.Nome = viewModel.Cliente.Nome;
                        clienteToUpdate.NomeFantasia = viewModel.Cliente.NomeFantasia;
                        clienteToUpdate.CodigoFiscal = viewModel.Cliente.CodigoFiscal;
                        clienteToUpdate.InscricaoEstatudal = viewModel.Cliente.InscricaoEstatudal;
                        clienteToUpdate.Endereco = viewModel.Cliente.Endereco;
                        clienteToUpdate.Numero = viewModel.Cliente.Numero;
                        clienteToUpdate.Bairro = viewModel.Cliente.Bairro;
                        clienteToUpdate.Cidade = viewModel.Cliente.Cidade;
                        clienteToUpdate.Estado = viewModel.Cliente.Estado;
                        clienteToUpdate.DataNascimento = viewModel.Cliente.DataNascimento;

                        if (viewModel.Imagem != null)
                            clienteToUpdate.ImagemUrl = viewModel.Cliente.ImagemUrl;
                    }
                }

                _dataService.SaveClients(allClients);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Erro ao salvar cliente: " + ex.Message);
                viewModel.Cidades = new SelectList(_dataService.GetCities(), viewModel.Cliente?.Cidade);
                return View("CreateEdit", viewModel);
            }
        }


        [HttpGet]
        public IActionResult Delete(int id)
        {
            var cliente = _dataService.GetClients().FirstOrDefault(c => c.ID == id);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var allClients = _dataService.GetClients();
            var clienteToDelete = allClients.FirstOrDefault(c => c.ID == id);

            if (clienteToDelete != null)
            {
                if (!string.IsNullOrEmpty(clienteToDelete.ImagemUrl))
                {
                    var imagePath = Path.Combine(_env.WebRootPath, clienteToDelete.ImagemUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath)) System.IO.File.Delete(imagePath);
                }
                
                allClients.Remove(clienteToDelete);
                _dataService.SaveClients(allClients);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult ExportJson(int id)
        {
            var cliente = _dataService.GetClients().FirstOrDefault(c => c.ID == id);
            if (cliente == null) return NotFound();

            var jsonString = JsonSerializer.Serialize(cliente, new JsonSerializerOptions { WriteIndented = true });
            return File(System.Text.Encoding.UTF8.GetBytes(jsonString), "application/json", $"cliente_{cliente.ID}.json");
        }
    }
}