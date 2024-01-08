global using AutoMapper;
namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        public CharacterService(IMapper mapper, DataContext context)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            if (await NameDuplicate(newCharacter.Name, 0))
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "There is already a character with that name";
                return serviceResponse;
            }
            var character = _mapper.Map<Character>(newCharacter);
            await _context.Characters.AddAsync(character);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
            serviceResponse.Success = true;
            serviceResponse.Message = "Character Added Successfully";
            return serviceResponse;
        }
        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacters(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            try
            {
                var objetoDominio = await _context.Characters.FindAsync(id) ?? throw new Exception($"Character with Id '{id}' not found");
                _context.Characters.Remove(objetoDominio);
                await _context.SaveChangesAsync();
                _context.ChangeTracker.Clear();
                List<Character> objetosDominio = await _context.Characters.ToListAsync();
                serviceResponse.Data = objetosDominio.Select(x => _mapper.Map<GetCharacterDto>(x)).ToList();
                serviceResponse.Success = true;
                serviceResponse.Message = "Character successfully deleted";
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;

        }
        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            List<Character> objectsDominio = await _context.Characters.ToListAsync();
            serviceResponse.Data = objectsDominio.Select(x => _mapper.Map<GetCharacterDto>(x)).ToList();
            return serviceResponse;
        }
        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();
            Character? character = await _context.Characters.FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (character is null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Character not found";
                return serviceResponse;
            }
            serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
            return serviceResponse;
        }
        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updateCharacterDto)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();
            try
            {
                var objetoDominio = await _context.Characters.FindAsync(updateCharacterDto.Id) ?? throw new Exception($"Character with Id '{updateCharacterDto.Id}' not found");
                if(objetoDominio.Name != updateCharacterDto.Name)
                {
                    if (await NameDuplicate(updateCharacterDto.Name, updateCharacterDto.Id))
                    {
                        serviceResponse.Success = false;
                        serviceResponse.Message = "There is already a character with that name";
                        return serviceResponse;
                    }
                }
                objetoDominio.Name = updateCharacterDto.Name;
                objetoDominio.HitPoints = updateCharacterDto.HitPoints;
                objetoDominio.Strength = updateCharacterDto.Strength;
                objetoDominio.Defense = updateCharacterDto.Defense;
                objetoDominio.Intelligence = updateCharacterDto.Intelligence;
                objetoDominio.Class = updateCharacterDto.Class;
                _context.Update(objetoDominio);
                await _context.SaveChangesAsync();
                _context.ChangeTracker.Clear();
                serviceResponse.Data = _mapper.Map<GetCharacterDto>(objetoDominio);

                serviceResponse.Success = true;
                serviceResponse.Message = "Character successfully updated";
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }
        private async Task<bool> NameDuplicate(string name, int id) =>
            await _context.Characters.Where(x=>x.Id != id).AnyAsync(x => x.Name.Equals(name)) ? true : false;
    }
}