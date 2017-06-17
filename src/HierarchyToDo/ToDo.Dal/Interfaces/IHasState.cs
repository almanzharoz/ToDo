namespace ToDo.Dal.Interfaces
{
	public enum ERecordState : ushort
	{
		New = 0b0000_0000, // новая
		Hidden = 0b0000_0001, // скрытая
		Deleted = 0b0000_0010, // удаленная
		Done = 0b0000_0100, // сделанная
		Anchored = 0b0000_1000, // закрепленная (нельзя изменить статус)

		Testing = 0b0001_0000, // в тестировании
		Tested = 0b0010_0000, // оттестирована и верна
		Return = 0b0100_0000, // отправлена на доработку
	}

	public interface IHasState
	{
		ERecordState State { get; set; }
	}
}