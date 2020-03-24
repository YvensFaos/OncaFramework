using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Onca
{
    public class GenerationManager : MonoBehaviour
    {
        [Header("Generators")]
        [SerializeField]
        private GenerateObjectsInArea[] objectsGenerators;
        [SerializeField]
        private GenerateAgentsInArea[] agentsGenerators;
        [SerializeField] 
        private Environment environment;

        [Space(10)]
        [Header("Parenting and Mutation")]
        [SerializeField]
        private float mutationFactor;
        [SerializeField] 
        private float mutationChance;
        [SerializeField] 
        private int parentSize;

        [Space(10)] 
        [Header("Simulation Controls")]
        [SerializeField, Tooltip("Time per simulation (in seconds).")]
        private float simulationTimer;
        [SerializeField, Tooltip("Current time spent on this simulation.")]
        private float simulationCount;
        [SerializeField, Tooltip("Automatically starts the simulation on Play.")]
        private bool runOnStart;
        [SerializeField, Tooltip("Initial count for the simulation. Used for the Prefabs naming.")]
        private int generationCount;

        [Space(10)] 
        [Header("Prefab Saving")]
        [SerializeField]
        private string savePrefabsAt;
        [SerializeField] 
        private string generationName;
    
        private bool _runningSimulation;
    
        private void Start()
        {
            if (runOnStart)
            {
                StartSimulation();
            }
        }
    
        private void Update()
        {
            if (_runningSimulation)
            {
                //Creates a new generation.
                if (simulationCount >= simulationTimer)
                {
                    ++generationCount;
                    MakeNewGeneration();
                    simulationCount = -Time.deltaTime;
                } 
                simulationCount += Time.deltaTime;
            }
        }

        public void GenerateObjects()
        {
            foreach (GenerateObjectsInArea generateObjectsInArea in objectsGenerators)
            {
                generateObjectsInArea.RegenerateObjects();
            }
        }

        public void GenerateAgents()
        {
            foreach (GenerateAgentsInArea agentGenerator in agentsGenerators)
            {
                agentGenerator.RegenerateObjects();
            }
        }
    
        /// <summary>
        /// Creates a new generation by using GenerateBoxes and GenerateBoats/Pirates.
        /// Previous generations will be removed and the best parents will be selected and used to create the new generation.
        /// The best parents (top 1) of the generation will be stored as a Prefab in the [savePrefabsAt] folder. Their name
        /// will use the [generationCount] as an identifier.
        /// </summary>
        public void MakeNewGeneration()
        {
            GenerateObjects();

            foreach (GenerateAgentsInArea agentGenerator in agentsGenerators)
            {
                List<AgentLogic> agentLogics = agentGenerator.GetActiveAgents();
                agentLogics.RemoveAll(logic => logic == null);
                agentLogics.ForEach(logic => logic.CalculatePerformance(environment));
                agentLogics.Sort();
            
                AgentLogic[] parents = new AgentLogic[parentSize];
                for (int i = 0; i < parentSize; i++)
                {
                    parents[i] = agentLogics[i];
                }

                AgentLogic lastWinner = parents[0];
                lastWinner.name += generationName + "[" + generationCount + "]";
                PrefabUtility.SaveAsPrefabAsset(lastWinner.gameObject, savePrefabsAt + lastWinner.name + ".prefab");
            
                List<AgentLogic> newAgents = agentGenerator.RegenerateObjects();
            
                newAgents.ForEach(agent =>
                {
                    AgentLogic parent = parents[Random.Range(0, parentSize)];
                    agent.Birth(parent.GetData());
                    agent.Mutate(mutationFactor, mutationChance);
                    agent.WakeUp();
                });
            }
        }

        /// <summary>
        /// Starts a new simulation. It does not call MakeNewGeneration. It calls both GenerateBoxes and GenerateObjects and
        /// then sets the _runningSimulation flag to true.
        /// </summary>
        public void StartSimulation()
        {
            GenerateObjects();
            GenerateAgents();
            _runningSimulation = true;
        }

        /// <summary>
        /// Continues the simulation. It calls MakeNewGeneration to use the previous state of the simulation and continue it.
        /// It sets the _runningSimulation flag to true.
        /// </summary>
        public void ContinueSimulation()
        {
            MakeNewGeneration();
            _runningSimulation = true;
        }
     
        /// <summary>
        /// Stops the count for the simulation. It also removes null (Destroyed) boats from the _activeBoats list and sets
        /// all boats and pirates to Sleep.
        /// </summary>
        public void StopSimulation()
        {
            _runningSimulation = false;

            foreach (GenerateAgentsInArea agentGenerator in agentsGenerators)
            {
                List<AgentLogic> agentLogics = agentGenerator.GetActiveAgents();
                agentLogics.RemoveAll(logic => logic == null);
                agentLogics.ForEach(logic => logic.Sleep());
            }
        }
    }
}
