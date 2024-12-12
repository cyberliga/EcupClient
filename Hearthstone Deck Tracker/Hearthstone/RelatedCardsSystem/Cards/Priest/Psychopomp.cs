﻿using System.Collections.Generic;
using System.Linq;

namespace Hearthstone_Deck_Tracker.Hearthstone.RelatedCardsSystem.Cards.Priest;

public class Psychopomp: ICardWithRelatedCards
{
	public string GetCardId() => HearthDb.CardIds.Collectible.Priest.Psychopomp;

	public bool ShouldShowForOpponent(Player opponent) => false;

	public List<Card?> GetRelatedCards(Player player) =>
		player.DeadMinionsCards
			.Select(entity => CardUtils.GetProcessedCardFromEntity(entity, player))
			.Where(card => card != null)
			.Distinct()
			.OrderByDescending(card => card!.Cost)
			.ToList();
}
