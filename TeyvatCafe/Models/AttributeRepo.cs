using System.Collections.Generic;
using System.Linq;

namespace TeyvatCafe.Models
{
    public class AttributeRepo
    {
        public static List<Attribute> Get10Attribute(DataContext context)
        {
            var output = new List<Attribute>().Take(10).ToList();
            
            return output;
        }
        public static Attribute AddAttribute(DataContext context, string attributeName2)
        {
            var newAttribute = new Attribute()
            {
                AttributeName = attributeName2,
                IsDelete = false, 
            };
            context.Attributes.Add(newAttribute);
            context.SaveChanges();
            return newAttribute;
        }
        public static bool EditAttribute(DataContext context, int attrID, string attrName)
        {
            var IdAttr = context.Attributes.FirstOrDefault(p => p.IdAttribute == attrID);
            if(IdAttr==null)
            {
                return false;
            }
            else
            {
                IdAttr.AttributeName = attrName;
                context.Attributes.Update(IdAttr);
                context.SaveChanges();
                return true;
            }    
        }
    }
}
