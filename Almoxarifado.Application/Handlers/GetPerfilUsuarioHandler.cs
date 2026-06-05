using MediatR;
using Google.Cloud.Firestore;
using Almoxarifado.Domain.Enums;

namespace Almoxarifado.Application.Queries;

public class GetPerfilUsuarioHandler : IRequestHandler<GetPerfilUsuarioQuery, PerfilUsuarioDto?>
{
    private readonly FirestoreDb _firestoreDb;

    public GetPerfilUsuarioHandler(FirestoreDb firestoreDb)
    {
        _firestoreDb = firestoreDb;
    }

    public async Task<PerfilUsuarioDto?> Handle(GetPerfilUsuarioQuery request, CancellationToken cancellationToken)
    {
        var docRef = _firestoreDb.Collection("Usuarios").Document(request.Uid);
        var snapshot = await docRef.GetSnapshotAsync(cancellationToken);

        if (!snapshot.Exists)
        {
            return null;
        }

        var nome = snapshot.TryGetValue("Nome", out string nomeStr) ? nomeStr : "Usuário";
        var email = snapshot.TryGetValue("Email", out string emailStr) ? emailStr : string.Empty;
        var setor = snapshot.TryGetValue("Setor", out string setorStr) ? setorStr : "Não Informado";

        var tipoNome = "Colaborador";
        if (snapshot.TryGetValue("Tipo", out int tipoInt))
        {
            if (Enum.IsDefined(typeof(TipoUsuario), tipoInt))
            {
                tipoNome = ((TipoUsuario)tipoInt).ToString();
            }
        }
        else if (snapshot.TryGetValue("Tipo", out string tipoStrFallback))
        {
            tipoNome = tipoStrFallback;
        }

        return new PerfilUsuarioDto
        {
            Id = request.Uid,
            Nome = nome,
            Email = email,
            Setor = setor,
            Tipo = tipoNome
        };
    }
}