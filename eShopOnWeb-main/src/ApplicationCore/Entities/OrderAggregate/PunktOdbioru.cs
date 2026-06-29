using System;
using Ardalis.GuardClauses;

namespace Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;

public class PunktOdbioru : IEquatable<PunktOdbioru>
{
    public string NazwaPunktu { get; private set; }
    public string Ulica { get; private set; }
    public string Miasto { get; private set; }
    public string KodPocztowy { get; private set; }

    #pragma warning disable CS8618 // Required by Entity Framework
    private PunktOdbioru() { }

    public PunktOdbioru(string nazwaPunktu, string ulica, string miasto, string kodPocztowy)
    {
        Guard.Against.NullOrEmpty(nazwaPunktu, nameof(nazwaPunktu));
        Guard.Against.NullOrEmpty(ulica, nameof(ulica));
        Guard.Against.NullOrEmpty(miasto, nameof(miasto));
        Guard.Against.NullOrEmpty(kodPocztowy, nameof(kodPocztowy));

        NazwaPunktu = nazwaPunktu;
        Ulica = ulica;
        Miasto = miasto;
        KodPocztowy = kodPocztowy;
    }

    public bool Equals(PunktOdbioru? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return NazwaPunktu == other.NazwaPunktu &&
               Ulica == other.Ulica &&
               Miasto == other.Miasto &&
               KodPocztowy == other.KodPocztowy;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as PunktOdbioru);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(NazwaPunktu, Ulica, Miasto, KodPocztowy);
    }

    public static bool operator ==(PunktOdbioru? left, PunktOdbioru? right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(PunktOdbioru? left, PunktOdbioru? right)
    {
        return !(left == right);
    }
}
