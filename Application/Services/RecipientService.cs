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



        public async Task<bool> ActualizarDestinatarioAsync(int id, UpdateDestinatarioDto dto)
        {
            // 1. Validar si existe el destinatario (Para el HTTP 404)
            var destinatario = await _recipientRepo.ObtenerPorId(id);
            if (destinatario == null)
            {
                throw new KeyNotFoundException("El destinatario no existe.");
            }

            // 2. Validar si enviaron un Distrito y si existe (Para el HTTP 400)
            if (dto.DistritoId.HasValue)
            {
                var distritoExiste = await _recipientRepo.ExisteDistrito(dto.DistritoId.Value);
                if (!distritoExiste)
                {
                    throw new ArgumentException("El distrito proporcionado no existe en el sistema.");
                }
                destinatario.DistritoId = dto.DistritoId.Value;
            }

            // 3. Actualizar solo los campos que vengan con información
            destinatario.Nombre = dto.Nombre ?? destinatario.Nombre;
            destinatario.Apellido = dto.Apellido ?? destinatario.Apellido;
            destinatario.Telefono = dto.Telefono ?? destinatario.Telefono;
            destinatario.Direccion = dto.Direccion ?? destinatario.Direccion;
            destinatario.Email = dto.Email ?? destinatario.Email;

            // 4. Guardar en BD (Asegúrate de tener un método Actualizar en tu repositorio)
            await _recipientRepo.Actualizar(destinatario);

            return true;
        }
    }
}