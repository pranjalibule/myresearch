using Newtonsoft.Json;

namespace SQS.nTier.TTM.GenericFramework.Utility
{
    public class Conversions

    {
        /// <summary>
        /// ToDTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <param name="objEntity"></param>
        /// <returns></returns>
        public static DTO ToDTO<DTO, Entity>(Entity objEntity) where DTO : IBaseDTO
        where Entity : IBaseEntity
        {

            var objEntityJSON = JsonConvert.SerializeObject(objEntity, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });

            DTO objDTO = JsonConvert.DeserializeObject<DTO>(objEntityJSON);

            //IFormatter formatter = new BinaryFormatter();
            //Stream stream = new MemoryStream();
            //using (stream)
            //{
            //    formatter.Serialize(stream, objEntity);
            //    stream.Seek(0, SeekOrigin.Begin);
            //    objDTO = (DTO)formatter.Deserialize(stream);
            //}

            return objDTO;
        }

        /// <summary>
        /// ToEntity
        /// </summary>
        /// <param name="objDTO"></param>
        /// <param name="objEntity"></param>
        /// <returns></returns>
        public static Entity ToEntity<DTO, Entity>(DTO objDTO) where DTO : IBaseDTO
        where Entity : IBaseEntity
        {

            var objDTOJSON = JsonConvert.SerializeObject(objDTO, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });

            //Entity objEntity = (Entity)Activator.CreateInstance(typeof(Entity));

            Entity objEntity = JsonConvert.DeserializeObject<Entity>(objDTOJSON);

            //IFormatter formatter = new BinaryFormatter();
            //Stream stream = new MemoryStream();
            //using (stream)
            //{
            //    formatter.Serialize(stream, objDTO);
            //    stream.Seek(0, SeekOrigin.Begin);
            //    objEntity = (Entity)formatter.Deserialize(stream);
            //}

            return objEntity;
        }
    }
}
