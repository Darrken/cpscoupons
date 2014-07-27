using CpsCouponsSolution.Models;

namespace CpsCouponsSolution.DTO
{
	public class FieldDTO
	{
		public FieldDTO()
		{}

		public FieldDTO(Program_Fields field)
		{
			Id = field.Id;
			Name = field.Name;
		}

		public int Id { get; set; }
		public string Name { get; set; }
	}

	public class FieldValueDTO
	{
		public FieldValueDTO()
		{}

		public FieldValueDTO(Program_Field_Values field)
		{
			Id = field.ProgramFieldId;
			RetailerId = field.ProgramRetailerId;
			Value = field.Value;
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public int? RetailerId { get; set; }
		public string Value { get; set; }
	}
}