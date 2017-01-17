using System;
using System.Linq.Expressions;
using Nest252;
using Tests.Framework;
using Tests.Framework.Integration;
using Tests.Framework.MockData;
using static Tests.Framework.RoundTripper;
using Xunit;
using FluentAssertions;

namespace Tests.ClientConcepts.HighLevel.Inference
{
	/**[[property-inference]]
	* == Property Name Inference
	*/
	public class PropertyNames : SimpleIntegration, IClusterFixture<WritableCluster>
	{
		private IElasticClient _client;

		public PropertyNames(WritableCluster cluster) : base(cluster)
		{
			_client = cluster.Client;
		}

		/**=== Appending suffixes to a Lambda expression body
		 * Suffixes can be appended to the body of a lambda expression, useful in cases where
		 * you have a POCO property mapped as a multi field
		 * and want to use strongly typed access based on the property, yet append a suffix to the
		 * generated field name in order to access a particular multi field.
		 *
		 * The `.Suffix()` extension method can be used for this purpose and when serializing expressions suffixed
		 * in this way, the serialized field name resolves to the last token
		 */
		[U] public void PropertyNamesAreResolvedToLastTokenUsingSuffix()
		{
			Expression<Func<Project, object>> expression = p => p.Name.Suffix("raw");
			Expect("raw").WhenSerializing<PropertyName>(expression);
		}

		/**=== Appending suffixes to a Lambda expression
		 * Alternatively, suffixes can be applied to a lambda expression directly using
		 * the `.ApplySuffix()` extension method. Again, the serialized field name
		 * resolves to the last token
		 */
		[U] public void PropertyNamesAreResolvedToLastTokenUsingAppendSuffix()
		{
			Expression<Func<Project, object>> expression = p => p.Name;
			expression = expression.AppendSuffix("raw");
			Expect("raw").WhenSerializing<PropertyName>(expression);
		}

		/**=== Naming conventions
		 * Currently, the name of a field cannot contain a `.` in Elasticsearch due to the potential for ambiguity with
		 * a field that is mapped as a multi field.
		 *
		 * In these cases, NEST allows the call to go to Elasticsearch, deferring the naming conventions to the server side and,
		 * in the case of a `.` in a field name, a `400 Bad Response` is returned with a server error indicating the reason
		 */
		[I] public void PropertyNamesContainingDotsCausesElasticsearchServerError()
		{
			var createIndexResponse = _client.CreateIndex("random-" + Guid.NewGuid().ToString().ToLowerInvariant(), c => c
				.Mappings(m => m
					.Map("type-with-dot", mm => mm
						.Properties(p => p
							.String(s => s
								.Name("name-with.dot")
							)
						)
					)
				)
			);

			/** The response is not valid */
			createIndexResponse.ShouldNotBeValid();

			/** `DebugInformation` provides an audit trail of information to help diagnose the issue */
			createIndexResponse.DebugInformation.Should().NotBeNullOrEmpty();

			/** `ServerError` contains information about the response from Elasticsearch */
			createIndexResponse.ServerError.Should().NotBeNull();
			createIndexResponse.ServerError.Status.Should().Be(400);
			createIndexResponse.ServerError.Error.Should().NotBeNull();
			createIndexResponse.ServerError.Error.RootCause.Should().NotBeNullOrEmpty();

			var rootCause = createIndexResponse.ServerError.Error.RootCause[0];

			/** We can see that the underlying reason is a `.` in the field name "name-with.dot" */
			rootCause.Reason.Should().Be("Field name [name-with.dot] cannot contain '.'");
			rootCause.Type.Should().Be("mapper_parsing_exception");
		}
	}
}
