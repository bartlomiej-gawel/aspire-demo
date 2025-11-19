<script setup lang="ts">
import type { Organization } from '../types'
import {
  getOrganizationStatusLabel,
  getOrganizationStatusVariant,
  getSubscriptionPlanLabel,
  getSubscriptionStatusLabel,
  getSubscriptionStatusVariant,
  formatDate,
} from '../lib/organization-utils'
import { Badge } from '@/components/ui/badge'
import { Separator } from '@/components/ui/separator'
import { Building2, CreditCard } from 'lucide-vue-next'
import { computed } from 'vue'

interface Props {
  organization: Organization
}

const props = defineProps<Props>()

const usedSeats = computed(() => {
  return props.organization.subscription.totalSeats - props.organization.subscription.availableSeats
})
</script>

<template>
  <div class="grid gap-6 md:grid-cols-2">
    <!-- Organization Information Card -->
    <div class="rounded-lg border bg-card p-6 space-y-4">
      <div class="flex items-center gap-2">
        <Building2 class="h-5 w-5 text-muted-foreground" />
        <h3 class="text-lg font-semibold">Organization Information</h3>
      </div>
      <Separator />
      <div class="space-y-3">
        <div class="flex justify-between items-center">
          <span class="text-sm text-muted-foreground">Name</span>
          <span class="text-sm font-medium">{{ organization.name }}</span>
        </div>
        <div class="flex justify-between items-center">
          <span class="text-sm text-muted-foreground">Status</span>
          <Badge :variant="getOrganizationStatusVariant(organization.status)">
            {{ getOrganizationStatusLabel(organization.status) }}
          </Badge>
        </div>
        <div class="flex justify-between items-center">
          <span class="text-sm text-muted-foreground">Employees</span>
          <span class="text-sm font-medium">{{ organization.employees.length }}</span>
        </div>
        <div class="flex justify-between items-center">
          <span class="text-sm text-muted-foreground">Locations</span>
          <span class="text-sm font-medium">{{ organization.locations.length }}</span>
        </div>
      </div>
    </div>

    <!-- Subscription Details Card -->
    <div class="rounded-lg border bg-card p-6 space-y-4">
      <div class="flex items-center gap-2">
        <CreditCard class="h-5 w-5 text-muted-foreground" />
        <h3 class="text-lg font-semibold">Subscription</h3>
      </div>
      <Separator />
      <div class="space-y-3">
        <div class="flex justify-between items-center">
          <span class="text-sm text-muted-foreground">Plan</span>
          <Badge variant="outline" class="font-semibold">
            {{ getSubscriptionPlanLabel(organization.subscription.plan) }}
          </Badge>
        </div>
        <div class="flex justify-between items-center">
          <span class="text-sm text-muted-foreground">Status</span>
          <Badge :variant="getSubscriptionStatusVariant(organization.subscription.status)">
            {{ getSubscriptionStatusLabel(organization.subscription.status) }}
          </Badge>
        </div>
        <div class="flex justify-between items-center">
          <span class="text-sm text-muted-foreground">Expires At</span>
          <span class="text-sm">{{ formatDate(organization.subscription.expiresAt) }}</span>
        </div>
        <Separator />
        <div class="flex justify-between items-center">
          <span class="text-sm text-muted-foreground">Total Seats</span>
          <span class="text-sm font-medium">{{ organization.subscription.totalSeats }}</span>
        </div>
        <div class="flex justify-between items-center">
          <span class="text-sm text-muted-foreground">Used Seats</span>
          <span class="text-sm font-medium">{{ usedSeats }}</span>
        </div>
        <div class="flex justify-between items-center">
          <span class="text-sm text-muted-foreground">Available Seats</span>
          <span class="text-sm font-medium text-primary">{{ organization.subscription.availableSeats }}</span>
        </div>
      </div>
    </div>

  </div>
</template>
