<script setup lang="ts">
import type { Organization } from '../types'
import {
  getEmployeeStatusLabel,
  getEmployeeStatusVariant,
  formatDate,
} from '../lib/organization-utils'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { UserCog } from 'lucide-vue-next'
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from '@/components/ui/tooltip'

interface Props {
  organization: Organization
}

defineProps<Props>()

function getRoleLabel(role: number): string {
  switch (role) {
    case 1:
      return 'Admin'
    case 2:
      return 'Manager'
    case 3:
      return 'Employee'
    default:
      return 'Unknown'
  }
}
</script>

<template>
  <div class="space-y-4">
    <div class="flex items-center justify-between">
      <div>
        <h3 class="text-lg font-semibold">Employee List</h3>
        <p class="text-sm text-muted-foreground">
          {{ organization.employees.length }} employee{{ organization.employees.length !== 1 ? 's' : '' }} in this organization
        </p>
      </div>
    </div>

    <div class="rounded-md border">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Name</TableHead>
            <TableHead>Email</TableHead>
            <TableHead>Phone</TableHead>
            <TableHead>Role</TableHead>
            <TableHead>Status</TableHead>
            <TableHead>Created At</TableHead>
            <TableHead class="w-[70px]">
              <span class="sr-only">Actions</span>
            </TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          <TableRow v-if="organization.employees.length === 0">
            <TableCell colspan="7" class="text-center text-muted-foreground h-32">
              No employees found
            </TableCell>
          </TableRow>
          <TableRow v-for="employee in organization.employees" :key="employee.id">
            <TableCell class="font-medium">
              {{ employee.firstName }} {{ employee.lastName }}
            </TableCell>
            <TableCell>
              <a :href="`mailto:${employee.email}`" class="text-primary hover:underline">
                {{ employee.email }}
              </a>
            </TableCell>
            <TableCell class="text-muted-foreground">
              {{ employee.phone || 'N/A' }}
            </TableCell>
            <TableCell>
              <Badge variant="outline">
                {{ getRoleLabel(employee.role) }}
              </Badge>
            </TableCell>
            <TableCell>
              <Badge :variant="getEmployeeStatusVariant(employee.status)">
                {{ getEmployeeStatusLabel(employee.status) }}
              </Badge>
            </TableCell>
            <TableCell class="text-muted-foreground">
              {{ formatDate(employee.createdAt) }}
            </TableCell>
            <TableCell>
              <TooltipProvider>
                <Tooltip>
                  <TooltipTrigger as-child>
                    <Button variant="ghost" size="sm">
                      <UserCog class="h-4 w-4" />
                    </Button>
                  </TooltipTrigger>
                  <TooltipContent>
                    <p>Impersonate user</p>
                  </TooltipContent>
                </Tooltip>
              </TooltipProvider>
            </TableCell>
          </TableRow>
        </TableBody>
      </Table>
    </div>
  </div>
</template>
