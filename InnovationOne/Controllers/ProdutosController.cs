using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InnovationOne.Models;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualBasic.CompilerServices;

namespace InnovationOne.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly Context _context;
        IHostingEnvironment _appEnvironment;

        public ProdutosController(Context context, IHostingEnvironment env)
        {
            _context = context;
            _appEnvironment = env;
        }

        // GET: Produtos
        public async Task<IActionResult> Index()
        {
            var context = _context.Produto.Include(p => p.Categoria);
            return View(await context.ToListAsync());
        }

        // GET: Produtos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produto
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        // GET: Produtos/Create
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "Id", "Descricao");
            return View();
        }

        // POST: Produtos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descricao,Quantidade,CategoriaId")] Produto produto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(produto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "Id", "Descricao", produto.CategoriaId);
            return View(produto);
        }

        // GET: Produtos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produto.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "Id", "Descricao", produto.CategoriaId);
            return View(produto);
        }

        // POST: Produtos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descricao,Quantidade,CategoriaId")] Produto produto)
        {
            if (id != produto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(produto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdutoExists(produto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "Id", "Descricao", produto.CategoriaId);
            return View(produto);
        }

        // GET: Produtos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produto
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produto = await _context.Produto.FindAsync(id);
            _context.Produto.Remove(produto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public ActionResult contador()
        {
            
            var produto = _context.Produto;
            var produtoTotal = 0;
            var count = produto.Count();
            foreach (var item in produto)
            {
                produtoTotal = produtoTotal + item.Quantidade;
            }
            var resultado = new
            {
                produtoN = count,
                produtoTotal = produtoTotal
            };
            return Json(resultado);


        }
       
        private bool ProdutoExists(int id)
        {
            return _context.Produto.Any(e => e.Id == id);
        }


        //importar o arquivo na tabela produto
        public async Task<string> ImportAsync(string filename)
        {
            string erro = "";
            int counter = 0;
            string line;
            string tline = "";
            StreamReader file = new System.IO.StreamReader(filename);
            while ((line = file.ReadLine()) != null)
            {
                try
                {
                    String[] strlist = line.Split(';');
                    Produto produto = new Produto();
                    /* {
                         Descricao = strlist[1],
                         Quantidade = Int32.Parse(strlist[3]),
                         CategoriaId = Int32.Parse(strlist[2])
                     };*/
                    produto.Descricao = strlist[1];
                    produto.Quantidade = (int)Int32.Parse(strlist[3]);
                    produto.CategoriaId = (int)Int32.Parse(strlist[2]);

                    if (ModelState.IsValid)
                    {
                        _context.Add(produto);
                        _ = await _context.SaveChangesAsync();
                    }
                    counter++;
                }
                catch 
                {
                    erro += $"A linha {Convert.ToString(counter)} esta com erro! \r\n";

                }
                
            }
            ViewData["Erro"] = erro;
            return $"Foram importados {Convert.ToString(counter)} produtos";
          
            file.Close();

        }

        //fazer upload do arquivo
        public async Task<IActionResult> EnviarArquivo(List<IFormFile> arquivos)
        {
            long tamanhoArquivos = arquivos.Sum(f => f.Length);
            
            var caminhoArquivo = Path.GetTempFileName();
            var mensagem = "";
            foreach (var arquivo in arquivos)
            {
                if (arquivo == null || arquivo.Length == 0)
                {
                    ViewData["Erro"] = "Error: Arquivo(s) não selecionado(s)";
                    return View(ViewData);
                }
                string pasta = "Arquivos_Usuario";
                string nomeArquivo = "Usuario_arquivo_" + DateTime.Now.Millisecond.ToString();
                nomeArquivo += ".csv";
                string caminho_WebRoot = _appEnvironment.WebRootPath;
                string caminhoDestinoArquivo = caminho_WebRoot + "\\Arquivos\\" + pasta + "\\";
                string caminhoDestinoArquivoOriginal = caminhoDestinoArquivo + "\\Recebidos\\" + nomeArquivo;
                using (var stream = new FileStream(caminhoDestinoArquivoOriginal, FileMode.Create))
                {
                    await arquivo.CopyToAsync(stream);
                }
                mensagem = await ImportAsync(caminhoDestinoArquivoOriginal);
            }
            ViewData["Resultado"] = mensagem;


            var context = _context.Produto.Include(p => p.Categoria);
           

            return View("../Produtos/Index", await context.ToListAsync());
        }
    }
}
