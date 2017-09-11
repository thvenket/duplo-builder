[	  
	  {
	        "EnvName": "default",
                "LocalFleet": "true", 
		"WorkFlow" : [  
					   {  
						  "Name":"PRE_DEPLOY_BUILD",
						  "PhaseType":4,
						  "BuildParams":"PHASE=PRE_DEPLOY_BUILD, FOO=BAR",
						  "Order":0,
						  "Parallelism":1,
						  "ContainerImage":"duplocloud/msbuild:v4",
						  "ContainerPlatform": "5" 
					   }
					]
	  }
]

