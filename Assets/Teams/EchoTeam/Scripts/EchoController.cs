using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DoNotModify;

namespace Echo
{
	public class EchoController : BaseSpaceShipController
	{
		private InputData _inputData;

		public override void Initialize(SpaceShipView spaceship, GameData data)
		{
		}

		public override InputData UpdateInput(SpaceShipView spaceship, GameData data)
		{
			InputData inputDataCopy = _inputData;

			// Reset booleans for next update
			_inputData.shoot = false;
			_inputData.dropMine = false;
			_inputData.fireShockwave = false;

			return inputDataCopy;
		}
		
		public ref InputData GetInputDataByRef() => ref _inputData;
	}

}
