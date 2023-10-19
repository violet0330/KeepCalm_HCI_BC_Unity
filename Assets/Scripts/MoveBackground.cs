using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeepCalm
{
	public class MoveBackground : MonoBehaviour
	{

		public float speed;
		private float x;
		public float PontoDeDestino;
		public float PontoOriginal;

		internal static float ratio = 1.2f;



		// Use this for initialization
		void Start()
		{ 
			//PontoOriginal = transform.position.x;
		}

		// Update is called once per frame
		void Update()
		{

			x = transform.position.x;
			x += speed * ratio * Time.deltaTime;
			transform.position = new Vector3(x, transform.position.y, transform.position.z);



			if (x <= PontoDeDestino)
			{


				x = PontoOriginal;
				transform.position = new Vector3(x, transform.position.y, transform.position.z);
			}


		}
	}
}