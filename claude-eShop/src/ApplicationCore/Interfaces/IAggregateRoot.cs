namespace ApplicationCore.Interfaces;

// Marker interface. Only types implementing this may be queried/persisted directly through
// a repository - internal entities of an aggregate must be reached via the aggregate root.
public interface IAggregateRoot
{
}
