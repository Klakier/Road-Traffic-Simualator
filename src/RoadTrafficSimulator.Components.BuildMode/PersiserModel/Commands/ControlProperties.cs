using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using RoadTrafficSimulator.Infrastructure;
using RoadTrafficSimulator.Infrastructure.Controls;

namespace RoadTrafficSimulator.Components.BuildMode.PersiserModel.Commands
{
    [Serializable]
    public class ControlProperties : IAction
    {
        private readonly Type _type;
        private readonly Guid _ownerId;
        private readonly MemberInfo[] _path;
        private readonly Guid _commandId = Guid.NewGuid();

        public static ControlProperties Create<TOwner, TProperty>( TOwner owner, TProperty propertyValue ) where TOwner : IControl
        {
            var property = owner.GetType().GetProperties().Where( s => s.PropertyType.IsInstanceOfType( propertyValue ) ).FirstOrDefault( s => s.GetValue( owner, null ) == ( object ) propertyValue );
            return new ControlProperties( typeof( TProperty ), owner.Id, new MemberInfo[] { property } );
        }

        public static ControlProperties Create<T>( Guid ownerId, Expression<Func<T>> expression )
        {
            var memeberExpression = expression.Body as MemberExpression;
            if ( memeberExpression == null ) { throw new ArgumentException(); }
            return new ControlProperties( typeof( T ), ownerId, new[] { memeberExpression.Member } );
        }

        public ControlProperties( Type type, Guid ownerId, MemberInfo[] path )
        {
            Contract.Requires( type != null );
            Contract.Requires( path != null );
            this._type = type;
            this._ownerId = ownerId;
            this._path = path;
        }

        public object Execute( DeserializationContext context )
        {
            object result = context.GetById( this._ownerId );

            return this._path.Aggregate( result, ( current, memberInfo ) => this.GetValue( memberInfo, current ) );
        }

        private object GetValue( MemberInfo memberInfo, object owner )
        {
            if ( memberInfo is PropertyInfo )
            {
                return ( ( PropertyInfo ) memberInfo ).GetValue( owner, null );
            }
            return ( ( FieldInfo ) memberInfo ).GetValue( owner );
        }

        public Order Priority
        {
            get { return Order.Low; }
        }

        public Type Type
        {
            get { return this._type; }
        }

        public Guid CommandId
        {
            get { return this._commandId; }
        }
    }
}