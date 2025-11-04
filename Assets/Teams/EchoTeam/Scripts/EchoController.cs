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
			return _inputData;
		}
		
		public ref InputData GetInputDataByRef() => ref _inputData;
	}

}
