using Application.DTOS;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class RecipientService : IRecipientService
    {
        private readonly IRecipientRepository _recipientRepo;

        public RecipientService(IRecipientRepository recipientRepo)
        {
            _recipientRepo = recipientRepo;
        }

        public async Task<Destinatario> CrearDestinatarioAsync(CreateDestinatarioDto dto)
        {
            // 1. Aquí vive la regla de negocio
            var distritoExiste = await _recipientRepo.ExisteDistrito(dto.DistritoId);
            if (!distritoExiste)
                throw new ArgumentException("El distrito no existe");

            // 2. Mapeamos el DTO a la entidad pura
            var destinatario = new Destinatario
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Telefono = dto.Telefono,
                Direccion = dto.Direccion,
                Email = dto.Email,
                DistritoId = dto.DistritoId
            };

            // 3. Delegamos el guardado al repositorio
            return await _recipientRepo.Crear(destinatario);
        }
    }
}