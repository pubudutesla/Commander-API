using System.Collections.Generic;
using AutoMapper;
using Commander.Data;
using Commander.Dtos;
using Commander.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Commander.Controllers
{
    //api/Commands
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommanderRepository _repository;
        private readonly IMapper _mapper;

        public CommandsController(ICommanderRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // GET api/commands
        [HttpGet]
        public ActionResult <IEnumerable<CommandReadDto>> GetAllCommands()
        {
            var  commandItems = _repository.GetAllCommands();
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commandItems));
        }

        // GET api/commands/2
        [HttpGet("{id}", Name="GetCommandById")]
        public ActionResult<CommandReadDto> GetCommandById(int id) // id value will come from route
        {
            var  commandItem = _repository.GetCommandById(id);
            if(commandItem != null)
            {
                return Ok(_mapper.Map<CommandReadDto>(commandItem));
            }
            else
            {
                return NotFound();
            }
        }

        //POST api/commands
        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommand(CommandCreateDto cmd)
        {
            if(ModelState.IsValid)
            {
                var commandModal = _mapper.Map<Command>(cmd);
                _repository.CreateCommand(commandModal);
                _repository.SaveChanges();

                var commandReadDto = _mapper.Map<CommandReadDto>(commandModal);

                return CreatedAtRoute(nameof(GetCommandById), new {Id = commandReadDto.Id}, commandReadDto);

            }
            else
            {
                return BadRequest();
            }
        }

        // PUT api/commands/2
        [HttpPut ("{id}")]
        public ActionResult UpdateCommand(int id, CommandUpdateDto commandUpdateDto)
        {
            var commandModalFromRepo = _repository.GetCommandById(id);
            if(commandModalFromRepo == null)
            {
                return NotFound();
            }
            
             _mapper.Map(commandUpdateDto, commandModalFromRepo);

             _repository.UpdateCommand(commandModalFromRepo);
             _repository.SaveChanges();

             return NoContent();
        }

        // PATCH api/commands/2
        [HttpPatch ("{id}")]
        public ActionResult PartialCommandUpdate(int id, JsonPatchDocument<CommandUpdateDto> patchDoc)
        {
            var commandModalFromrepo = _repository.GetCommandById(id);
            if(commandModalFromrepo == null)
            {
                return NotFound();
            }

            var commandToPatch = _mapper.Map<CommandUpdateDto> (commandModalFromrepo);
             patchDoc.ApplyTo(commandToPatch, ModelState);

             if(!TryValidateModel(commandToPatch))
             {
                 return ValidationProblem(ModelState);
             }
             
             _mapper.Map(commandToPatch, commandModalFromrepo);

             _repository.UpdateCommand(commandModalFromrepo);
             _repository.SaveChanges();

             return NoContent();
        }

        // DELETE api/commands/2
        [HttpDelete ("{id}")]
        public ActionResult DeleteCommand(int id)
        {
            var commandModalFromrepo = _repository.GetCommandById(id);
            if(commandModalFromrepo == null)
            {
                return NotFound();
            }

            _repository.DeleteCommand(commandModalFromrepo);
            _repository.SaveChanges();

            return NoContent();
        }
    }
}