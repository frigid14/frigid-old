﻿using Content.Shared.Construction;
using JetBrains.Annotations;
using Robust.Shared.Containers;

namespace Content.Server.Construction.Completions
{
    [UsedImplicitly]
    [DataDefinition]
    public sealed class EmptyAllContainers : IGraphAction
    {
        public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
        {
            if (!entityManager.TryGetComponent(uid, out ContainerManagerComponent? containerManager))
                return;

            var transform = entityManager.GetComponent<TransformComponent>(uid);
            foreach (var container in containerManager.GetAllContainers())
            {
                container.EmptyContainer(true, transform.Coordinates);
            }
        }
    }
}
