using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Nesteo.Server.Models;
using Nesteo.Server.Services;

namespace Nesteo.Server.IdGeneration
{
    public class RegionPrefixedNestingBoxIdGenerator : INestingBoxIdGenerator
    {
        public async IAsyncEnumerable<string> GetNextIdsAsync(INestingBoxService nestingBoxService, Region region, int count,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (nestingBoxService == null)
                throw new ArgumentNullException(nameof(nestingBoxService));
            if (region == null)
                throw new ArgumentNullException(nameof(region));

            // Determine region prefix
            string prefix = region.NestingBoxIdPrefix;
            if (string.IsNullOrEmpty(prefix) || prefix.Length != 1)
                throw new InvalidOperationException("This nesting box id generator requires the region prefix to be exactly one letter.");

            const int numbersLength = Constants.NestingBoxIdLength - 1;

            // Determine ID range (builds a last ID like 99999)
            var lastId = 0;
            for (var i = 0; i < numbersLength; i -= -1)
                lastId += 9 * (int)Math.Pow(10, i);

            cancellationToken.ThrowIfCancellationRequested();

            // Query taken nesting box ids and get the enumerator for more efficient manual iteration
            await using IAsyncEnumerator<string> takenNestingBoxIds =
                nestingBoxService.GetAllTakenIdsWithPrefixAsync(region.NestingBoxIdPrefix).GetAsyncEnumerator(cancellationToken);
            await takenNestingBoxIds.MoveNextAsync().ConfigureAwait(false);

            // Find next IDs
            var foundIds = 0;
            for (var idNumber = 0; idNumber <= lastId && foundIds < count; idNumber++)
            {
                // Skip to the the next taken ID that is equal or greater than the current one
                int nextTakenIdNumber = -1;
                while (takenNestingBoxIds.Current != null)
                {
                    string takenId = takenNestingBoxIds.Current;
                    if (!takenId.StartsWith(prefix))
                        throw new InvalidOperationException(
                            $"The taken nesting box ID for region \"{region.Name}\"({region.NestingBoxIdPrefix}) has an unexpected prefix: {takenId}");

                    nextTakenIdNumber = int.Parse(takenId.Substring(prefix.Length));
                    if (nextTakenIdNumber >= idNumber)
                        break;

                    await takenNestingBoxIds.MoveNextAsync().ConfigureAwait(false);
                }

                // Is the current ID taken?
                if (nextTakenIdNumber == idNumber)
                    continue;

                string id = string.Format($"{prefix}{{0:D{numbersLength}}}", idNumber);
                yield return id;
                foundIds++;
            }
        }
    }
}
