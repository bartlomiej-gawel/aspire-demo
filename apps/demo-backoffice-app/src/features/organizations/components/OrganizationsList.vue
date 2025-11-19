<script setup lang="ts">
import { ref } from 'vue'
import type { Organization, CreateOrganizationData } from '../types'
import { OrganizationStatus, OrganizationEmployeeStatus, OrganizationEmployeeRole, OrganizationLocationStatus } from '../types'
import { mockOrganizations } from '../data/mock-organizations'
import OrganizationFormSheet from './OrganizationFormSheet.vue'
import {
  getOrganizationStatusLabel,
  getOrganizationStatusVariant,
  getSubscriptionPlanLabel,
  getSubscriptionStatusLabel,
  getSubscriptionStatusVariant,
} from '../lib/organization-utils'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { Plus, MoreVertical, Eye, Archive } from 'lucide-vue-next'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/components/ui/alert-dialog'

const organizations = ref<Organization[]>(mockOrganizations)
const isDialogOpen = ref(false)
const isArchiveDialogOpen = ref(false)
const organizationToArchive = ref<Organization | null>(null)

function handleCreateNew() {
  isDialogOpen.value = true
}

function handleArchiveClick(organization: Organization) {
  organizationToArchive.value = organization
  isArchiveDialogOpen.value = true
}

function handleArchiveConfirm() {
  if (organizationToArchive.value) {
    const index = organizations.value.findIndex(org => org.id === organizationToArchive.value!.id)
    if (index !== -1) {
      const org = organizations.value[index]
      if (org) {
        org.status = OrganizationStatus.Archived
        org.updatedAt = new Date().toISOString()
      }
    }
  }
  isArchiveDialogOpen.value = false
  organizationToArchive.value = null
}

function handleSave(data: CreateOrganizationData) {
  const organizationId = crypto.randomUUID()
  const now = new Date().toISOString()
  const trialExpiresAt = new Date()
  trialExpiresAt.setDate(trialExpiresAt.getDate() + 14)

  const newOrganization: Organization = {
    id: organizationId,
    name: data.organizationName,
    subscription: {
      plan: 4, // Platinum
      status: 1, // Trial
      expiresAt: trialExpiresAt.toISOString(),
      totalSeats: 50,
      availableSeats: 50 - data.organizationAdmins.length,
    },
    status: OrganizationStatus.Inactive,
    createdAt: now,
    updatedAt: null,
    locations: [
      {
        id: crypto.randomUUID(),
        organizationId,
        name: `${data.organizationName} Headquarters`,
        openingHours: {
          weekly: {
            0: { from: '09:00', to: '17:00', isEnabled: false },
            1: { from: '09:00', to: '17:00', isEnabled: true },
            2: { from: '09:00', to: '17:00', isEnabled: true },
            3: { from: '09:00', to: '17:00', isEnabled: true },
            4: { from: '09:00', to: '17:00', isEnabled: true },
            5: { from: '09:00', to: '17:00', isEnabled: true },
            6: { from: '10:00', to: '14:00', isEnabled: true },
          },
        },
        address: null,
        status: OrganizationLocationStatus.Active,
        createdAt: now,
        updatedAt: null,
      },
    ],
    employees: data.organizationAdmins.map(admin => ({
      id: crypto.randomUUID(),
      organizationId,
      firstName: admin.firstName,
      lastName: admin.lastName,
      email: admin.email,
      phone: admin.phone,
      status: OrganizationEmployeeStatus.Inactive,
      role: OrganizationEmployeeRole.Admin,
      createdAt: now,
      updatedAt: null,
    })),
  }

  organizations.value.push(newOrganization)
}
</script>

<template>
  <div class="flex flex-1 flex-col gap-4">
    <div class="flex items-center justify-between">
      <div>
        <h2 class="text-2xl font-bold tracking-tight">
          Organizations
        </h2>
        <p class="text-muted-foreground">
          Manage organizations in the system
        </p>
      </div>
      <Button @click="handleCreateNew">
        <Plus class="mr-2 h-4 w-4" />
        Add Organization
      </Button>
    </div>

    <div class="rounded-md border">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Name</TableHead>
            <TableHead>Status</TableHead>
            <TableHead>Subscription</TableHead>
            <TableHead>Plan</TableHead>
            <TableHead>Employees</TableHead>
            <TableHead>Locations</TableHead>
            <TableHead class="w-[70px]">
              <span class="sr-only">Actions</span>
            </TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          <TableRow v-if="organizations.length === 0">
            <TableCell colspan="7" class="text-center text-muted-foreground">
              No organizations found
            </TableCell>
          </TableRow>
          <TableRow v-for="organization in organizations" :key="organization.id">
            <TableCell class="font-medium">
              {{ organization.name }}
            </TableCell>
            <TableCell>
              <Badge :variant="getOrganizationStatusVariant(organization.status)">
                {{ getOrganizationStatusLabel(organization.status) }}
              </Badge>
            </TableCell>
            <TableCell>
              <Badge :variant="getSubscriptionStatusVariant(organization.subscription.status)">
                {{ getSubscriptionStatusLabel(organization.subscription.status) }}
              </Badge>
            </TableCell>
            <TableCell>
              <Badge variant="outline">
                {{ getSubscriptionPlanLabel(organization.subscription.plan) }}
              </Badge>
            </TableCell>
            <TableCell>{{ organization.employees.length }}</TableCell>
            <TableCell>{{ organization.locations.length }}</TableCell>
            <TableCell>
              <DropdownMenu>
                <DropdownMenuTrigger as-child>
                  <Button variant="ghost" size="sm">
                    <MoreVertical class="h-4 w-4" />
                  </Button>
                </DropdownMenuTrigger>
                <DropdownMenuContent align="end">
                  <DropdownMenuItem as-child>
                    <a :href="`#details/${organization.id}`">
                      <Eye class="mr-2 h-4 w-4" />
                      View Details
                    </a>
                  </DropdownMenuItem>
                  <DropdownMenuSeparator />
                  <DropdownMenuItem
                    :disabled="organization.status === OrganizationStatus.Archived"
                    @click="handleArchiveClick(organization)"
                  >
                    <Archive class="mr-2 h-4 w-4" />
                    Archive
                  </DropdownMenuItem>
                </DropdownMenuContent>
              </DropdownMenu>
            </TableCell>
          </TableRow>
        </TableBody>
      </Table>
    </div>

    <OrganizationFormSheet
      v-model:open="isDialogOpen"
      @save="handleSave"
    />

    <AlertDialog v-model:open="isArchiveDialogOpen">
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Archive Organization</AlertDialogTitle>
          <AlertDialogDescription>
            Are you sure you want to archive "{{ organizationToArchive?.name }}"?
            This action will mark the organization as archived and it will no longer be active.
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel>Cancel</AlertDialogCancel>
          <AlertDialogAction @click="handleArchiveConfirm">
            Archive
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  </div>
</template>
