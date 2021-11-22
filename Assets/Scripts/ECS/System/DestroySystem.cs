using Unity.Collections;
using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Minecraft
{
    public class DestroySystem : SystemBase
    {
        private EntityCommandBufferSystem bufferSystem;
        private EntityQuery blockQuery;
        private EntityQuery destoryBlockQuery;
        private EntityQuery surfacePlantQuery;
        protected override void OnCreate()
        {
            bufferSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EntityCommandBufferSystem>();
            this.blockQuery = GetEntityQuery(typeof(Translation), typeof(BlockTag));
            this.destoryBlockQuery = GetEntityQuery(typeof(Translation), typeof(DestroyTag));
            this.surfacePlantQuery = GetEntityQuery(typeof(Translation), typeof(SurfacePlantTag));
        }
        protected override void OnUpdate()
        {

            var blocks = this.blockQuery.ToEntityArray(Allocator.TempJob);
            var destoryBlocks = this.destoryBlockQuery.ToEntityArray(Allocator.TempJob);
            var surfacePlants = this.surfacePlantQuery.ToEntityArray(Allocator.TempJob);
            var positions = this.GetComponentDataFromEntity<Translation>();

            foreach (var destoryBlock in destoryBlocks)
            {
                foreach (var block in blocks)
                {
                    Vector3 offset = positions[block].Value - positions[destoryBlock].Value;
                    var sqrLen = offset.sqrMagnitude;
                    //find the block to destroy
                    if (sqrLen == 0)
                    {
                        //remove the plant from the surface;
                        foreach (var sufacePlant in surfacePlants)
                        {
                            var sufacePlantPos = positions[sufacePlant].Value;
                            var tmpPos = new float3(sufacePlantPos.x, sufacePlantPos.y + Vector3.down.y, sufacePlantPos.z);
                            offset = positions[block].Value - tmpPos;
                            sqrLen = offset.sqrMagnitude;
                            if (sqrLen == 0)
                            {
                                this.EntityManager.DestroyEntity(sufacePlant);
                            }
                        }
                        //remove blocks
                        this.EntityManager.DestroyEntity(destoryBlock);
                        this.EntityManager.DestroyEntity(block);
                    }
                }
            }
            blocks.Dispose();
            destoryBlocks.Dispose();
            surfacePlants.Dispose();


            //for (int i = 0; i < sourceBlock.Length; i++)
            //{
            //    for (int j = 0; j < targetBlocks.Length; j++)
            //    {
            //        Vector3 offset = targetBlocks.positions[j].Value - sourceBlock.positions[i].Value;
            //        float sqrLen = offset.sqrMagnitude;

            //        //find the block to destroy
            //        if (sqrLen == 0)
            //        {
            //            //remove the plant from the surface;
            //            for (int k = 0; k < surfaceplants.Length; k++)
            //            {
            //                float3 tmpPos = new float3(surfaceplants.positions[k].Value.x, surfaceplants.positions[k].Value.y + Vector3.down.y, surfaceplants.positions[k].Value.z);
            //                offset = targetBlocks.positions[j].Value - tmpPos;
            //                sqrLen = offset.sqrMagnitude;

            //                if (sqrLen == 0)
            //                {
            //                    this.EntityManager.DestroyEntity(surfaceplants.entity[k]);
            //                }
            //            }

            //            //remove blocks
            //            this.EntityManager.DestroyEntity(sourceBlock.entity[i]);
            //            this.EntityManager.DestroyEntity(targetBlocks.entity[j]);
            //        }
            //    }
            //}
        }

        //protected override void OnDestroy()
        //{
        //    this.blockQuery.Dispose();
        //    this.destoryBlockQuery.Dispose();
        //    this.surfacePlantQuery.Dispose();
        //}
    }
}
